using Domain.Enums;
using System.Collections.Generic;
using System;

namespace Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime FechaRegistro { get; set; }
        public Rol Rol { get; set; }
        public ICollection<ParticipanteViaje> ParticipantesViaje { get; set; } = new List<ParticipanteViaje>();
        public Usuario(string nombre, string email, string password)
        {
            Nombre = nombre;
            Email = email;
            Password = password;
            FechaRegistro = DateTime.Now;
            Rol = Rol.User;

        }

    }
}



