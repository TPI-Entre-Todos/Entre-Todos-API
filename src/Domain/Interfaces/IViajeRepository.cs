using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IViajeRepository
    {
        List<Viaje> GetAll();
        public Viaje GetById(int id);

        public Viaje Add(Viaje entity);

        public void Delete(int id);
    }
}