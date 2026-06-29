using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDetalleGastoRepository : IGenericRepository<DetalleGasto>
    {
        List<DetalleGasto> GetByGastoId(int gastoId);
        List<DetalleGasto> GetByParticipanteId(int participanteId);
        DetalleGasto Add(DetalleGasto entity);
        DetalleGasto Update(DetalleGasto entity);
        void AddRange(List<DetalleGasto> entities);
        void Delete(int id);
    }
}
