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

        /// <summary>Identificador estable del usuario en Cognito (claim "sub"). Único.</summary>
        public string? CognitoSub { get; set; }

        /// <summary>Preparado para foto de perfil, fase 2.</summary>
        public string? AvatarUrl { get; set; }

        public DateTime FechaRegistro { get; set; }
        public Rol Rol { get; set; }
        public ICollection<ParticipanteViaje> ParticipantesViaje { get; set; } = new List<ParticipanteViaje>();

        public Usuario(string nombre, string email, string cognitoSub)
        {
            Nombre = nombre;
            Email = email;
            CognitoSub = cognitoSub;
            FechaRegistro = DateTime.Now;
            Rol = Rol.User;
        }

        public Usuario()
        {

        }
    }
}
