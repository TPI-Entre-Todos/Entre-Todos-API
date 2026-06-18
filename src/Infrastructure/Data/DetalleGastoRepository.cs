using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DetalleGastoRepository : IDetalleGastoRepository
    {
        private readonly ApplicationContext _context;

        public DetalleGastoRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<DetalleGasto> GetByIdAsync(int id)
        {
            return await _context.DetallesGasto.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<DetalleGasto>> GetByGastoIdAsync(int gastoId)
        {
            return await _context.DetallesGasto
                .Where(d => d.GastoId == gastoId)
                .ToListAsync();
        }

        public async Task<List<DetalleGasto>> GetByParticipanteIdAsync(int participanteId)
        {
            return await _context.DetallesGasto
                .Where(d => d.ParticipanteId == participanteId)
                .ToListAsync();
        }

        public async Task<DetalleGasto> AddAsync(DetalleGasto entity)
        {
            await _context.DetallesGasto.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task AddRangeAsync(List<DetalleGasto> entities)
        {
            await _context.DetallesGasto.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.DetallesGasto.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}