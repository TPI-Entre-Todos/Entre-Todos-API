using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface INotificacionRepository
    {
        Notificacion GetById(int id);
        List<Notificacion> GetByUsuarioId(int usuarioId);
        Notificacion Add(Notificacion entity);
        void Update(Notificacion entity);
    }
}