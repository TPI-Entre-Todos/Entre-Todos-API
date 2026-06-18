using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
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

        public List<ViajeDto> Get()
        {

            var viajes = _viajeRepository.GetAll();
            return ViajeDto.CreateList(viajes);
        }

        public ViajeDto? GetById(int id)
        {
            var viaje = _viajeRepository.GetById(id);
            return viaje == null ? null : ViajeDto.Create(viaje);
        }

        public void Delete(int id)
        {
            _viajeRepository.Delete(id);
        }
    }
}