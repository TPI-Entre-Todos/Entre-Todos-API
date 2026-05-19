using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        string? Autenticar(AuthenticationRequest authenticationRequest);
    }
}