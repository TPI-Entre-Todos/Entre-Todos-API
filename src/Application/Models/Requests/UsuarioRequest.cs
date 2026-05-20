using Domain.Entities;

namespace Application.Models.Requests
{
    public class UsuarioRequest
    {
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

    }
}
