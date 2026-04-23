using Domain.Entities;
namespace Domain.Interfaces
{
    public interface IViajeService
    {
        void Add(Viaje viaje);
        List<Viaje> Get();
        Viaje GetById(int id);
        public void Delete(int id);
    }
}