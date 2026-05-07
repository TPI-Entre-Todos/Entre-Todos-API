using Domain.Entities;
using Domain.Interfaces;


namespace Application.Services
{
    public class ViajeService : IViajeService
    {

        private readonly IViajeRepository _viajeRepository;
        public ViajeService(IViajeRepository viajeRepository)
        { _viajeRepository = viajeRepository; }
        public Viaje Add(Viaje viaje)
        {
            //viaje.Id = _viajes.Count + 1;
            _viajeRepository.Add(viaje);
            return viaje;
        }

        public List<Viaje> Get()
        {
            return _viajeRepository.GetAll();
        }

        public Viaje GetById(int id)
        {
            return _viajeRepository.GetById(id);
        }

        public void Delete(int id)
        {
            _viajeRepository.Delete(id);
        }
    }
}