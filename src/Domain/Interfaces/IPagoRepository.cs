using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPagoRepository : IGenericRepository<Pago>
    {
        List<Pago> GetByViajeId(int viajeId);
        List<Pago> GetByParticipanteId(int participanteId);
    }
}
