
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Models;
using System.Text.RegularExpressions;

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

        public UsuarioDto Add(UsuarioRequest request)
        {
            ValidarSolicitudUsuario(request);

            var email = request.Email!.Trim();
            if (_usuarioRepository.GetUserByEmail(email) != null)
                throw new BadRequestException("Ya existe un usuario con ese email.");

            var usuario = new Usuario(
                request.Nombre!.Trim(),
                email,
                request.Password!.Trim()
            );

            _usuarioRepository.Add(usuario);
            return UsuarioDto.Create(usuario);
        }

        public UsuarioDto Update(int id, UsuarioRequest request)
        {
            if (id <= 0)
                throw new BadRequestException("Id de usuario inválido.");

            if (request == null)
                throw new BadRequestException("Solicitud de actualización inválida.");

            var existing = _usuarioRepository.GetById(id);
            if (existing == null)
                throw new NotFoundException("Usuario no encontrado.");

            if (string.IsNullOrWhiteSpace(request.Nombre)
                && string.IsNullOrWhiteSpace(request.Email)
                && string.IsNullOrWhiteSpace(request.Password))
            {
                throw new BadRequestException("No se proporcionaron campos para actualizar.");
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var email = request.Email.Trim();
                if (!ValidarEmail(email))
                    throw new BadRequestException("Email inválido.");

                var usuarioConEmail = _usuarioRepository.GetUserByEmail(email);
                if (usuarioConEmail != null && usuarioConEmail.Id != id)
                    throw new BadRequestException("El email ya está en uso por otro usuario.");

                existing.Email = email;
            }

            if (!string.IsNullOrWhiteSpace(request.Nombre))
                existing.Nombre = request.Nombre.Trim();

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var password = request.Password.Trim();
                if (password.Length < 6)
                    throw new BadRequestException("La contraseña debe tener al menos 6 caracteres.");

                existing.Password = password;
            }

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

        public Usuario GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Email inválido.");

            return _usuarioRepository.GetUserByEmail(email.Trim());
        }

        private static void ValidarSolicitudUsuario(UsuarioRequest request)
        {
            if (request == null)
                throw new BadRequestException("Solicitud de usuario inválida.");

            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new BadRequestException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new BadRequestException("El email es obligatorio.");

            if (!ValidarEmail(request.Email))
                throw new BadRequestException("El email no es válido.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new BadRequestException("La contraseña es obligatoria.");

            if (request.Password.Trim().Length < 6)
                throw new BadRequestException("La contraseña debe tener al menos 6 caracteres.");
        }

        private static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var correo = email.Trim();
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(correo);
        }
    }
}
