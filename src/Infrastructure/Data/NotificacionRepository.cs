using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Data
{
    public class NotificacionRepository : GenericRepository<Notificacion>, INotificacionRepository
    {
        public NotificacionRepository(ApplicationContext context) : base(context)
        {
        }

        public List<Notificacion> GetByUsuarioId(int usuarioId)
        {
            return _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha)
                .ToList();
        }
    }
}
