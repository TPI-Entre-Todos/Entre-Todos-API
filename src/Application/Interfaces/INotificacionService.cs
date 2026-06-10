using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;

namespace Application
{
    public interface INotificacionService
    {
        Task<NotificacionDto> CrearNotificacionAsync(NotificacionCreateDto dto);
        Task<List<NotificacionDto>> ObtenerPorUsuarioAsync(int usuarioId);
        Task MarcarComoLeidaAsync(int id);
    }
}