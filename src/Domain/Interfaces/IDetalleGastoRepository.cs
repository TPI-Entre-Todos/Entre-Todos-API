using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDetalleGastoRepository : IGenericRepository<DetalleGasto>
    {
        List<DetalleGasto> GetByGastoId(int gastoId);
        List<DetalleGasto> GetByParticipanteId(int participanteId);
        void AddRange(List<DetalleGasto> entities);
    }
}
