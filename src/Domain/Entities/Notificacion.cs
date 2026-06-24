using System;

namespace Domain.Entities
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; } // A quién va dirigida
                                           // Opcional, si la notificación está relacionada con un viaje 
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }

        // Propiedad de navegación
        public Usuario? Usuario { get; set; }// Si la notificación está relacionada con un viaje

        public Notificacion(int usuarioId, string mensaje)
        {
            UsuarioId = usuarioId;
            Mensaje = mensaje;
            Fecha = DateTime.Now;
            Leida = false;
        }
    }
}