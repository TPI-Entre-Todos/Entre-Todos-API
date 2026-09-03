
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
        private const string ContenedorAvatares = "avatars";

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IParticipanteViajeRepository _participanteViajeRepository;
        private readonly IFileStorageService _fileStorageService;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IParticipanteViajeRepository participanteViajeRepository,
            IFileStorageService fileStorageService)
        {
            _usuarioRepository = usuarioRepository;
            _participanteViajeRepository = participanteViajeRepository;
            _fileStorageService = fileStorageService;
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

        /// <summary>
        /// Un usuario sólo puede eliminarse a sí mismo, salvo que sea Admin.
        /// </summary>
        public void Delete(int id, int usuarioAutenticadoId, bool esAdmin)
        {
            if (id <= 0)
                throw new BadRequestException("Id de usuario inválido.");

            if (!esAdmin && id != usuarioAutenticadoId)
                throw new Domain.Exceptions.UnauthorizedAccessException("Sólo podés eliminar tu propia cuenta.");

            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                throw new NotFoundException("Usuario no encontrado.");

            // Borrar el usuario arrastra sus ParticipanteViaje por cascada, salteando la
            // validación de saldos de EliminarParticipante. Peor: gastos y pagos apuntan al
            // participante con FK Restrict, así que la cascada fallaría con un error de base
            // en vez de un mensaje entendible. Como todo el historial financiero cuelga de
            // ParticipanteViaje, exigir que no participe en ningún viaje es la única condición
            // que garantiza un borrado limpio.
            var participaciones = _participanteViajeRepository.GetByUsuarioId(id);
            if (participaciones.Count > 0)
                throw new BadRequestException(
                    $"No se puede eliminar el usuario: participa en {participaciones.Count} viaje(s). " +
                    "Primero hay que darlo de baja de cada viaje, lo que a su vez exige que no tenga saldos pendientes.");

            _usuarioRepository.Delete(id);
        }

        public async Task<UsuarioDto> ActualizarAvatarAsync(
            int id,
            Stream contenido,
            long tamanio,
            int usuarioAutenticadoId,
            bool esAdmin,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new BadRequestException("Id de usuario inválido.");

            if (!esAdmin && id != usuarioAutenticadoId)
                throw new Domain.Exceptions.UnauthorizedAccessException("Sólo podés cambiar tu propia foto de perfil.");

            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                throw new NotFoundException("Usuario no encontrado.");

            var extension = ValidadorImagen.ValidarYObtenerExtension(contenido, tamanio);

            // Nombre aleatorio: si se usara el Id del usuario, las fotos serían adivinables
            // y enumerables, y el bucket es de lectura pública.
            var nombreArchivo = $"{Guid.NewGuid():N}{extension}";

            var urlAnterior = usuario.AvatarUrl;

            var urlNueva = await _fileStorageService.SubirAsync(
                contenido,
                nombreArchivo,
                ValidadorImagen.ContentTypePara(extension),
                ContenedorAvatares,
                cancellationToken);

            usuario.AvatarUrl = urlNueva;
            _usuarioRepository.Update(usuario);

            // El archivo viejo se borra recién con la nueva foto ya subida y persistida: si
            // fallara antes, el usuario quedaría apuntando a un archivo inexistente. Un borrado
            // fallido acá sólo deja un huérfano en el bucket, que es el error más barato.
            if (!string.IsNullOrWhiteSpace(urlAnterior))
            {
                try
                {
                    await _fileStorageService.EliminarAsync(urlAnterior, ContenedorAvatares, cancellationToken);
                }
                catch
                {
                    // Ignorado a propósito: el avatar nuevo ya quedó guardado y funcionando.
                }
            }

            return UsuarioDto.Create(usuario);
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
