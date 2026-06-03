using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGastoRepository
    {
        Task<Gasto> GetByIdAsync(int id);
        Task<List<Gasto>> GetByViajeIdAsync(int viajeId);
        Task<Gasto> AddAsync(Gasto entity);
        Task UpdateAsync(Gasto entity);
        Task DeleteAsync(int id);
    }
}