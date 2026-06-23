using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDetalleGastoRepository
    {
        Task<DetalleGasto> GetByIdAsync(int id);
        Task<List<DetalleGasto>> GetByGastoIdAsync(int gastoId);
        Task<List<DetalleGasto>> GetByParticipanteIdAsync(int participanteId);
        Task<DetalleGasto> AddAsync(DetalleGasto entity);
        Task AddRangeAsync(List<DetalleGasto> entities); // Para guardar la división de una sola vez
        Task DeleteAsync(int id);
    }
}