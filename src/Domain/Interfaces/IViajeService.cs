using Domain.Entities;
namespace Domain.Interfaces
{
    public interface IViajeService
    {
        Viaje Add(Viaje viaje);
        List<Viaje> Get();
        Viaje GetById(int id);
        public void Delete(int id);
    }
}