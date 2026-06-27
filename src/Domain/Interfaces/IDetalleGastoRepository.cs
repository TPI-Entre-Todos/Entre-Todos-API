using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDetalleGastoRepository
    {
        DetalleGasto GetById(int id);
        List<DetalleGasto> GetByGastoId(int gastoId);
        List<DetalleGasto> GetByParticipanteId(int participanteId);
        DetalleGasto Add(DetalleGasto entity);
        void AddRange(List<DetalleGasto> entities); // Para guardar la división de una sola vez
        void Delete(int id);
    }
}