using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class GastoRepository : IGastoRepository
    {
        private readonly ApplicationContext _context;

        public GastoRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Gasto GetById(int id)
        {
            return _context.Gastos
                .FirstOrDefault(g => g.Id == id);
        }

        public List<Gasto> GetByViajeId(int viajeId)
        {
            return _context.Gastos
                .Where(g => g.ViajeId == viajeId)
                .ToList();
        }

        public Gasto Add(Gasto entity)
        {
            _context.Gastos.Add(entity);
            _context.SaveChangesAsync();
            return entity;
        }

        public Gasto Update(Gasto entity)
        {
            _context.Gastos.Update(entity);
            _context.SaveChangesAsync();
            return entity;
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.Gastos.Remove(entity);
                _context.SaveChangesAsync();
            }
        }
    }
}