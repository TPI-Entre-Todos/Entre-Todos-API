using Application.Interfaces;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly AutenticacionServiceOptions _options;

        public AuthenticationService(IUsuarioRepository usuarioRepository, IOptions<AutenticacionServiceOptions> options)
        {
            _usuarioRepository = usuarioRepository;
            _options = options.Value;
        }
        private Usuario? ValidateUser(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrEmpty(authenticationRequest.Email) || string.IsNullOrEmpty(authenticationRequest.Password))
                return null;

            var user = _usuarioRepository.GetUserByEmail(authenticationRequest.Email);
            if (user == null) return null;
            // ✅ Validar que la contraseña coincida
            if (user.Password != authenticationRequest.Password)
                return null;
            return user;

        }
        public string Autenticar(AuthenticationRequest authenticationRequest)
        {
            var user = ValidateUser(authenticationRequest);
            if (user is not null)
            {
                // Paso 2: Crear el token
                var secretKey = _options.SecretForKey ?? throw new InvalidOperationException("Authentication secret key no está configurada.");
                var securityPassword = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
                var credentials = new SigningCredentials(securityPassword, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Nombre),
                    new Claim(ClaimTypes.Role, user.Rol.ToString())
                };
                var jwtToken = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

                var tokenToReturn = new JwtSecurityTokenHandler() //Pasamos el token a string
                .WriteToken(jwtToken);

                return tokenToReturn.ToString();


            }

            return null;

        }


    }
    public class AutenticacionServiceOptions
    {
        public const string AutenticacionService = "AutenticacionService";

        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretForKey { get; set; }
    }

}


