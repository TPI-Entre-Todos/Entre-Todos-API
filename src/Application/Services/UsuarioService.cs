
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
            if (string.IsNullOrEmpty(request.Email))
                throw new BadRequestException("El email es requerido.");

            var usuarioExistente = _usuarioRepository.GetUserByEmail(request.Email);
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

            if (!string.IsNullOrEmpty(request.Nombre))
                existing.Nombre = request.Nombre;
            if (!string.IsNullOrEmpty(request.Email))
            {
                if (existing.Email != request.Email)
                {
                    var usuarioExistente = _usuarioRepository.GetUserByEmail(request.Email);
                    if (usuarioExistente != null)
                        throw new BadRequestException("El email ya está registrado.");
                }
                existing.Email = request.Email;
            }
            if (!string.IsNullOrEmpty(request.Password))
                existing.Password = request.Password;

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
            _usuarioRepository.Delete(id);
        }

        public Usuario GetUserByEmail(string email)
        {
            return _usuarioRepository.GetUserByEmail(email);
        }
    }
}
