using Domain.Entities;
using Domain.Interfaces;


namespace Application.Services
{
    public class ViajeService : IViajeService
    {

        private readonly static List<Viaje> _viajes = [];

        public void AgregarViaje(Viaje viaje)
        {
            //viaje.Id = _viajes.Count + 1;
            viaje.Id = _viajes.Count > 0 ? _viajes.Max(v => v.Id) + 1 : 0;
            _viajes.Add(viaje);
        }

        public List<Viaje> ObtenerViajes()
        {
            return _viajes;
        }

        public void EliminarViaje(int id)
        {
            var viaje = _viajes.FirstOrDefault(v => v.Id == id);
            if (viaje != null)
            {
                _viajes.Remove(viaje);
            }
        }
    }
}