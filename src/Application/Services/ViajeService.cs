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

        private readonly IViajeRepository _viajeRepository;

        private readonly IParticipanteViajeRepository _participanteViajeRepository;
        public ViajeService(IViajeRepository viajeRepository, IParticipanteViajeRepository participanteViajeRepository)
        {
            _viajeRepository = viajeRepository;
            _participanteViajeRepository = participanteViajeRepository;
        }

        public ViajeDto Add(ViajeRequest request, int userIdClaim)
        {
            ValidarViaje(request);

            var viaje = new Viaje(
                request.Nombre!,
                request.Descripcion!,
                request.Moneda!
            );

            _viajeRepository.Add(viaje);

            var participante = new ParticipanteViaje(userIdClaim, viaje.Id, true);
            _participanteViajeRepository.Add(participante);

            return ViajeDto.Create(viaje);
        }

        public List<ViajeDto> Get(int userId, bool esAdmin)
        {
            var viajes = _viajeRepository.GetAll();

            if (!esAdmin)
            {
                // Obtener solo los IDs de viajes donde participa este usuario
                var viajesDelUsuario = _participanteViajeRepository.GetByUsuarioId(userId)
                    .Select(pv => pv.ViajeId)
                    .ToList();

                viajes = viajes.Where(v => viajesDelUsuario.Contains(v.Id)).ToList();
            }

            return ViajeDto.CreateList(viajes);
        }

        public ViajeDto? GetById(int id, int userId, bool esAdmin)
        {
            var viaje = _viajeRepository.GetById(id);
            if (viaje == null)
                throw new NotFoundException("Viaje no encontrado.");

            if (!esAdmin)
            {
                // Verificar que el usuario es participante del viaje
                var participante = _participanteViajeRepository.GetByIds(userId, id);
                if (participante == null)
                    throw new UnauthorizedException("No estás autorizado para ver este viaje.");
            }

            return ViajeDto.Create(viaje);
        }

        public void Delete(int id, int userId, bool esAdmin)
        {
            var viaje = _viajeRepository.GetById(id);
            if (viaje == null)
                throw new NotFoundException("Viaje no encontrado.");

            if (!esAdmin)
            {
                var participante = _participanteViajeRepository.GetByIds(userId, id);
                if (participante == null || !participante.EsOrganizador)
                    throw new UnauthorizedException("Solo administradores u organizadores pueden eliminar el viaje.");
            }

            _viajeRepository.Delete(id);
        }

        private static void ValidarViaje(ViajeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new BadRequestException("El nombre del viaje es obligatorio.");
            if (request.Nombre.Length > 100)
                throw new BadRequestException("El nombre no puede superar los 100 caracteres.");
            if (string.IsNullOrWhiteSpace(request.Descripcion))
                throw new BadRequestException("La descripción del viaje es obligatoria.");
            if (string.IsNullOrWhiteSpace(request.Moneda))
                throw new BadRequestException("La moneda del viaje es obligatoria.");
        }
    }
}
