using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPagoRepository
    {
        List<Pago> GetAll();
        Pago GetById(int id);
        Pago Add(Pago entity);
        Pago Update(Pago entity);
        void Delete(int id);
        List<Pago> GetByViajeId(int viajeId);
        List<Pago> GetByParticipanteId(int participanteId);
    }
}
