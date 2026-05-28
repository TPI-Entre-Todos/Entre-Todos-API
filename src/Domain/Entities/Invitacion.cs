using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domain.Entities
{

    public class Invitacion
    {
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public EstadoInvitacion Estado { get; set; } = EstadoInvitacion.Pendiente;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public int ViajeId { get; set; }
        public Viaje? Viaje { get; set; }
        public int UsuarioInvitadorId { get; set; }
        public Usuario? UsuarioInvitador { get; set; }
        public string EmailInvitado { get; set; } = string.Empty;
        public DateTime? FechaRespuesta { get; set; }


        public Invitacion(int viajeId, int usuarioInvitadorId, string emailInvitado, DateTime fechaExpiracion)
        {
            Token = Guid.NewGuid().ToString();
            Estado = EstadoInvitacion.Pendiente;
            FechaCreacion = DateTime.UtcNow;
            FechaExpiracion = fechaExpiracion;
            ViajeId = viajeId;
            UsuarioInvitadorId = usuarioInvitadorId;
            EmailInvitado = emailInvitado;
        }



    }

}