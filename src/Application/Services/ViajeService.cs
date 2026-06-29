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

            return ViajeDto.CreateList(viajes);
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

            return ViajeDto.Create(viaje);
        }

        public void Delete(int id)  
        {
            if (id <= 0)
                throw new BadRequestException("Id de viaje inválido.");

            var viaje = _viajeRepository.GetById(id);
            if (viaje == null)
                throw new NotFoundException("Viaje no encontrado.");

            _viajeRepository.Delete(id);
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
