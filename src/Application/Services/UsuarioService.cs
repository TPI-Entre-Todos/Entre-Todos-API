
using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Models;

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
            return UsuarioDto.Create(usuario);
        }

        public UsuarioDto Add(UsuarioRequest request)
        {
            Usuario usuario = new(request.Nombre, request.Email, request.Password);
            _usuarioRepository.Add(usuario);
            return UsuarioDto.Create(usuario);
        }

        public UsuarioDto Update(int id, UsuarioRequest request)
        {
            Usuario? existing = _usuarioRepository.GetById(id);
            if (existing != null)
            {
                if (!string.IsNullOrEmpty(request.Nombre))
                    existing.Nombre = request.Nombre;
                if (!string.IsNullOrEmpty(request.Email))
                    existing.Email = request.Email;
                if (!string.IsNullOrEmpty(request.Password))
                    existing.Password = request.Password;
            }
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