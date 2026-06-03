using System.Collections.Generic;
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

        public async Task<Gasto> GetByIdAsync(int id)
        {
            return await _context.Gastos
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Gasto>> GetByViajeIdAsync(int viajeId)
        {
            return await _context.Gastos
                .Where(g => g.ViajeId == viajeId)
                .ToListAsync();
        }

        public async Task<Gasto> AddAsync(Gasto entity)
        {
            await _context.Gastos.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Gasto entity)
        {
            _context.Gastos.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Gastos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}