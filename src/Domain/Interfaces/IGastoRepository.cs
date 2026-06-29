using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGastoRepository : IGenericRepository<Gasto>
    {
        List<Gasto> GetByViajeId(int viajeId);
        Gasto AddWithDetalles(Gasto gasto, Dictionary<int, decimal> saldoChanges);
        Gasto UpdateWithDetalles(Gasto gasto, Dictionary<int, decimal> saldoChanges);
        void DeleteWithSaldoReversal(int id);
    }
}
