using System.ComponentModel.DataAnnotations;

namespace Application.Models.Requests
{
    public class AuthenticationRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}