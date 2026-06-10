using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface INotificacionRepository
    {
        Task<Notificacion> GetByIdAsync(int id);
        Task<List<Notificacion>> GetByUsuarioIdAsync(int usuarioId);
        Task<Notificacion> AddAsync(Notificacion entity);
        Task UpdateAsync(Notificacion entity);
    }
}