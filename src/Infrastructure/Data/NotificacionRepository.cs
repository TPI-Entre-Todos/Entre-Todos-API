using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly ApplicationContext _context;

        public NotificacionRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Notificacion> GetByIdAsync(int id)
        {
            return await _context.Notificaciones.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<List<Notificacion>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha) // Las más nuevas primero
                .ToListAsync();
        }

        public async Task<Notificacion> AddAsync(Notificacion entity)
        {
            await _context.Notificaciones.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Notificacion entity)
        {
            _context.Notificaciones.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}