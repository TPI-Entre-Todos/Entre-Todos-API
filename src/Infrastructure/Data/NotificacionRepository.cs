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

        public Notificacion GetById(int id)
        {
            return _context.Notificaciones.FirstOrDefault(n => n.Id == id);
        }

        public List<Notificacion> GetByUsuarioId(int usuarioId)
        {
            return _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha) // Las más nuevas primero
                .ToList();
        }

        public Notificacion Add(Notificacion entity)
        {
            _context.Notificaciones.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(Notificacion entity)
        {
            _context.Notificaciones.Update(entity);
            _context.SaveChanges();
        }
    }
}