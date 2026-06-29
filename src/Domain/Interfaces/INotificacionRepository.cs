using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface INotificacionRepository : IGenericRepository<Notificacion>
    {
        List<Notificacion> GetByUsuarioId(int usuarioId);
    }
}
