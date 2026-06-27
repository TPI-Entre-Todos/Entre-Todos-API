using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGastoRepository
    {
        Gasto GetById(int id);
        List<Gasto> GetAll();
        List<Gasto> GetByViajeId(int viajeId);
        Gasto Add(Gasto entity);
        Gasto AddWithDetalles(Gasto gasto, Dictionary<int, decimal> saldoChanges);
        Gasto UpdateWithDetalles(Gasto gasto, Dictionary<int, decimal> saldoChanges);
        void Delete(int id);
        void DeleteWithSaldoReversal(int id);
    }
}