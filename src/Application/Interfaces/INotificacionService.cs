using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;

namespace Application
{
    public interface INotificacionService
    {
        NotificacionDto CrearNotificacion(NotificacionCreateDto dto);
        List<NotificacionDto> ObtenerPorUsuario(int usuarioId);
        void MarcarComoLeida(int id);
    }
}