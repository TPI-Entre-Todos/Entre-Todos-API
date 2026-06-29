
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Models;
using Domain.Enums;

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
            List<Usuario> usuarios = _usuarioRepository.GetAll();

            return UsuarioDto.CreateList(usuarios);
        }

        public UsuarioDto GetById(int id)
        {
            Usuario? usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                throw new NotFoundException("Usuario no encontrado.");

            return UsuarioDto.Create(usuario);
        }

        public UsuarioDto Add(UsuarioRequest request)
        {
            ValidarUsuarioParaCreacion(request);

            var usuarioExistente = _usuarioRepository.GetUserByEmail(request.Email!);
            if (usuarioExistente != null)
                throw new BadRequestException("El email ya está registrado.");

            Usuario usuario = new(request.Nombre, request.Email, request.Password);
            _usuarioRepository.Add(usuario);
            return UsuarioDto.Create(usuario);
        }

        public UsuarioDto Update(int id, UsuarioRequest request)
        {
            Usuario? existing = _usuarioRepository.GetById(id);
            if (existing == null)
                throw new NotFoundException("Usuario no encontrado.");

            if (string.IsNullOrWhiteSpace(request.Nombre) && string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.Password))
                throw new BadRequestException("Debe enviar al menos un campo para actualizar.");

            if (!string.IsNullOrWhiteSpace(request.Nombre))
                existing.Nombre = request.Nombre;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (!request.Email.Contains('@') || !request.Email.Contains('.'))
                    throw new BadRequestException("El formato del email no es válido.");
                if (existing.Email != request.Email)
                {
                    var usuarioExistente = _usuarioRepository.GetUserByEmail(request.Email);
                    if (usuarioExistente != null)
                        throw new BadRequestException("El email ya está registrado.");
                }
                existing.Email = request.Email;
            }
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (request.Password.Length < 6)
                    throw new BadRequestException("La contraseña debe tener al menos 6 caracteres.");
                existing.Password = request.Password;
            }

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

            existing.Rol = rol;
            return UsuarioDto.Create(_usuarioRepository.Update(existing));
        }

        public void Delete(int id)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                throw new NotFoundException("Usuario no encontrado.");

            _usuarioRepository.Delete(id);
        }

        private static void ValidarUsuarioParaCreacion(UsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new BadRequestException("El nombre es requerido.");
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new BadRequestException("El email es requerido.");
            if (!request.Email.Contains('@') || !request.Email.Contains('.'))
                throw new BadRequestException("El formato del email no es válido.");
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new BadRequestException("La contraseña es requerida.");
            if (request.Password.Length < 6)
                throw new BadRequestException("La contraseña debe tener al menos 6 caracteres.");
        }

        public Usuario GetUserByEmail(string email)
        {
            return _usuarioRepository.GetUserByEmail(email);
        }
    }
}
