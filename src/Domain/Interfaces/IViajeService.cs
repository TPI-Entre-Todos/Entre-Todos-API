using Domain.Entities;
namespace Domain.Interfaces
{
    public interface IViajeService
    {
        void AgregarViaje(Viaje viaje);
        List<Viaje> ObtenerViajes();
        public void EliminarViaje(int id);
    }
}