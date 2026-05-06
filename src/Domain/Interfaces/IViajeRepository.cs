using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IViajeRepository
    {
        public Viaje GetById(int id);

        public Viaje Add(Viaje entity);

        public void Delete(int id);
    }
}