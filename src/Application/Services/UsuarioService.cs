
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Models;
using Domain.Enums;
using System.Security.Claims;

namespace Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public List<UsuarioDto> GetAll()
        {
            var usuarios = _usuarioRepository.GetAll();
            return UsuarioDto.CreateList(usuarios);
        }

        public UsuarioDto GetById(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Id de usuario inválido.");

            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                throw new NotFoundException("Usuario no encontrado.");

            return UsuarioDto.Create(usuario);
        }

        /// <summary>
        /// El email y la contraseña los administra Cognito: acá sólo se edita el nombre.
        /// Un usuario únicamente puede editarse a sí mismo, salvo que sea Admin.
        /// </summary>
        public UsuarioDto Update(int id, UsuarioRequest request, int usuarioAutenticadoId, bool esAdmin)
        {
            if (id <= 0)
                throw new BadRequestException("Id de usuario inválido.");

            if (request == null)
                throw new BadRequestException("Solicitud de actualización inválida.");

            if (!esAdmin && id != usuarioAutenticadoId)
                throw new Domain.Exceptions.UnauthorizedAccessException("Sólo podés editar tu propio perfil.");

            var existing = _usuarioRepository.GetById(id);
            if (existing == null)
                throw new NotFoundException("Usuario no encontrado.");

            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new BadRequestException("El nombre es obligatorio.");

            existing.Nombre = request.Nombre.Trim();

            return UsuarioDto.Create(_usuarioRepository.Update(existing));
        }

        public UsuarioDto CambiarRol(int id, Rol rol)
        {
            Usuario? existing = _usuarioRepository.GetById(id);
            if (existing == null)
                throw new NotFoundException("Usuario no encontrado.");

            if (!Enum.IsDefined(rol))
                throw new BadRequestException("Rol inválido. Los roles válidos son: User (0), Admin (1).");

            if (existing.Rol == rol)
                throw new BadRequestException($"El usuario ya tiene asignado el rol {rol}.");

            // TODO (fase 2): si en algún momento la autorización pasa a apoyarse en los grupos
            // de Cognito, acá habría que replicar el cambio con AdminAddUserToGroup /
            // AdminRemoveUserFromGroup. Hoy el rol es exclusivamente nuestro, así que no aplica.
            existing.Rol = rol;
            return UsuarioDto.Create(_usuarioRepository.Update(existing));
        }

        public void Delete(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Id de usuario inválido.");

            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                throw new NotFoundException("Usuario no encontrado.");

            _usuarioRepository.Delete(id);
        }

        public UsuarioDto GetOrCreateFromToken(ClaimsPrincipal principal)
        {
            var cognitoSub = principal.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(cognitoSub))
                throw new UnauthorizedException("El token no contiene el claim 'sub'.");

            // Caso normal: el usuario ya fue dado de alta en un login anterior.
            var existente = _usuarioRepository.GetByCognitoSub(cognitoSub);
            if (existente != null)
                return UsuarioDto.Create(existente);

            var email = principal.FindFirst("email")?.Value?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                throw new UnauthorizedException("El token no contiene un email válido.");

            // Usuarios que ya existían antes de la migración a Cognito (por ejemplo el admin
            // inicial): se los adopta vinculándolos por email en vez de crear un duplicado,
            // así conservan su rol y todas sus relaciones con viajes, gastos y pagos.
            var porEmail = _usuarioRepository.GetUserByEmail(email);
            if (porEmail != null)
            {
                porEmail.CognitoSub = cognitoSub;
                return UsuarioDto.Create(_usuarioRepository.Update(porEmail));
            }

            var nombre = principal.FindFirst("name")?.Value
                ?? principal.FindFirst("cognito:username")?.Value
                ?? email;

            var usuario = new Usuario(nombre.Trim(), email, cognitoSub);

            _usuarioRepository.Add(usuario);
            return UsuarioDto.Create(usuario);
        }
    }
}
