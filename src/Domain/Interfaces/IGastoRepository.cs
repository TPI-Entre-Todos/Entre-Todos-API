using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IGastoRepository
    {
        Gasto GetById(int id);
        List<Gasto> GetByViajeId(int viajeId);
        Gasto Add(Gasto entity);
        Gasto Update(Gasto entity);
        void Delete(int id);
    }
}