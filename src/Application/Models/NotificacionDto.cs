using Domain.Entities;
namespace Application.Models
{
    public class NotificacionDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }
        public static NotificacionDto Create(Notificacion notificacion)
        {
            var dto = new NotificacionDto
            {
                Id = notificacion.Id,
                UsuarioId = notificacion.UsuarioId,
                Mensaje = notificacion.Mensaje,
                Fecha = notificacion.Fecha,
                Leida = notificacion.Leida
            };
            return dto;
        }
        public static List<NotificacionDto> CreateList(List<Notificacion> notificaciones)
        {
            var dtos = new List<NotificacionDto>();
            foreach (var notificacion in notificaciones)
            {
                dtos.Add(Create(notificacion));
            }
            return dtos;
        }
    }
}