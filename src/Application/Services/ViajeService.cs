using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class ViajeService : IViajeService
    {
        private const string ContenedorPortadas = "portadas";
        private const long TamanioMaximoPortadaBytes = 5 * 1024 * 1024; // 5 MB
        private static readonly TimeSpan VigenciaUrlPortada = TimeSpan.FromMinutes(15);

        private readonly IViajeRepository _viajeRepository;
        private readonly IParticipanteViajeRepository _participanteViajeRepository;
        private readonly IFileStorageService _fileStorageService;

        public ViajeService(
            IViajeRepository viajeRepository,
            IParticipanteViajeRepository participanteViajeRepository,
            IFileStorageService fileStorageService)
        {
            _viajeRepository = viajeRepository;
            _participanteViajeRepository = participanteViajeRepository;
            _fileStorageService = fileStorageService;
        }

        public ViajeDto Add(ViajeRequest request, int userIdClaim)
        {
            ValidarSolicitudDeViaje(request, userIdClaim);

            var viaje = new Viaje(
                request.Nombre!.Trim(),
                request.Descripcion!.Trim(),
                request.Moneda!.Trim()
            );

            _viajeRepository.Add(viaje);

            var participante = new ParticipanteViaje(userIdClaim, viaje.Id, true);
            _participanteViajeRepository.Add(participante);

            return ViajeDto.Create(viaje);
        }

        public List<ViajeDto> Get(int userId, bool esAdmin)
        {
            if (userId <= 0)
                throw new UnauthorizedException("Usuario no autenticado.");

            var viajes = _viajeRepository.GetAll();

            if (!esAdmin)
            {
                var viajesDelUsuario = _participanteViajeRepository.GetByUsuarioId(userId)
                    .Select(pv => pv.ViajeId)
                    .ToList();

                viajes = viajes.Where(v => viajesDelUsuario.Contains(v.Id)).ToList();
            }

            // La URL firmada se genera acá, no antes: quien llega a este punto ya pasó el
            // filtro de participante/Admin de arriba, así que es el único que se la lleva.
            return viajes.Select(v => ViajeDto.Create(v, ObtenerUrlPortada(v))).ToList();
        }

        public ViajeDto? GetById(int id, int userId, bool esAdmin)
        {
            if (id <= 0)
                throw new BadRequestException("Id de viaje inválido.");

            if (userId <= 0)
                throw new UnauthorizedException("Usuario no autenticado.");

            var viaje = _viajeRepository.GetById(id);
            if (viaje == null)
                throw new NotFoundException("Viaje no encontrado.");

            if (!esAdmin)
            {
                var participante = _participanteViajeRepository.GetByIds(userId, id);
                if (participante == null)
                    throw new Domain.Exceptions.UnauthorizedAccessException("No pertenecés a este viaje.");
            }

            return ViajeDto.Create(viaje, ObtenerUrlPortada(viaje));
        }

        /// <summary>
        /// Sólo puede borrar el viaje el organizador o un Admin.
        /// </summary>
        public async Task DeleteAsync(int id, int usuarioAutenticadoId, bool esAdmin, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new BadRequestException("Id de viaje inválido.");

            var viaje = _viajeRepository.GetById(id);
            if (viaje == null)
                throw new NotFoundException("Viaje no encontrado.");

            if (!esAdmin)
            {
                var participante = _participanteViajeRepository.GetByIds(usuarioAutenticadoId, id);
                if (participante == null || !participante.EsOrganizador)
                    throw new Domain.Exceptions.UnauthorizedAccessException(
                        "Sólo el organizador del viaje puede eliminarlo.");
            }

            _viajeRepository.Delete(id);

            // Se borra después de confirmar el Delete: si el borrado del registro fallara,
            // no queremos habernos quedado sin la portada de un viaje que sigue existiendo.
            if (!string.IsNullOrWhiteSpace(viaje.PortadaKey))
            {
                try
                {
                    await _fileStorageService.EliminarPorKeyAsync(viaje.PortadaKey, ContenedorPortadas, cancellationToken);
                }
                catch
                {
                    // Ignorado a propósito: el viaje ya se borró: un huérfano en el bucket
                    // privado es el error más barato, y nadie más puede ni leerlo.
                }
            }
        }

        /// <summary>
        /// Reemplaza la foto de portada. Sólo puede hacerlo el organizador del viaje o un Admin.
        /// </summary>
        public async Task<ViajeDto> ActualizarPortadaAsync(
            int id,
            Stream contenido,
            long tamanio,
            int usuarioAutenticadoId,
            bool esAdmin,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new BadRequestException("Id de viaje inválido.");

            var viaje = _viajeRepository.GetById(id);
            if (viaje == null)
                throw new NotFoundException("Viaje no encontrado.");

            if (!esAdmin)
            {
                var participante = _participanteViajeRepository.GetByIds(usuarioAutenticadoId, id);
                if (participante == null || !participante.EsOrganizador)
                    throw new Domain.Exceptions.UnauthorizedAccessException(
                        "Sólo el organizador del viaje puede cambiar la portada.");
            }

            var extension = ValidadorImagen.ValidarYObtenerExtension(contenido, tamanio, TamanioMaximoPortadaBytes);

            // Nombre aleatorio: aunque el bucket es privado, no hay razón para que la key
            // sea adivinable (Id del viaje, por ejemplo).
            var nombreArchivo = $"{Guid.NewGuid():N}{extension}";

            var keyAnterior = viaje.PortadaKey;

            var keyNueva = await _fileStorageService.SubirPrivadoAsync(
                contenido,
                nombreArchivo,
                ValidadorImagen.ContentTypePara(extension),
                ContenedorPortadas,
                cancellationToken);

            viaje.PortadaKey = keyNueva;
            _viajeRepository.Update(viaje);

            if (!string.IsNullOrWhiteSpace(keyAnterior))
            {
                try
                {
                    await _fileStorageService.EliminarPorKeyAsync(keyAnterior, ContenedorPortadas, cancellationToken);
                }
                catch
                {
                    // Ignorado a propósito: la portada nueva ya quedó guardada y funcionando.
                }
            }

            return ViajeDto.Create(viaje, ObtenerUrlPortada(viaje));
        }

        private string? ObtenerUrlPortada(Viaje viaje)
        {
            return string.IsNullOrWhiteSpace(viaje.PortadaKey)
                ? null
                : _fileStorageService.ObtenerUrlFirmada(viaje.PortadaKey, ContenedorPortadas, VigenciaUrlPortada);
        }

        private static void ValidarSolicitudDeViaje(ViajeRequest request, int userIdClaim)
        {
            if (request == null)
                throw new BadRequestException("Solicitud de viaje inválida.");

            if (userIdClaim <= 0)
                throw new UnauthorizedException("Usuario no autenticado.");

            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new BadRequestException("El nombre del viaje es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Descripcion))
                throw new BadRequestException("La descripción del viaje es obligatoria.");

            if (string.IsNullOrWhiteSpace(request.Moneda))
                throw new BadRequestException("La moneda del viaje es obligatoria.");
        }
    }
}
