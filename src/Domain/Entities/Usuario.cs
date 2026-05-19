using Domain.Enums;

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



