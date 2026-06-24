using System;

namespace Domain.Entities
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; } // A quién va dirigida
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }

        // Propiedad de navegación
        public Usuario Usuario { get; set; }
    }
}