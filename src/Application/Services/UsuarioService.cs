
using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
using Application.Models.Requests;

namespace Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public List<Usuario> GetAll()
        {
            return _usuarioRepository.GetAll();
        }

        public Usuario GetById(int id)
        {
            return _usuarioRepository.GetById(id);
        }

        public Usuario Add(UsuarioRequest request)
        {
            Usuario usuario = new(request.Nombre, request.Email, request.Password);

            return _usuarioRepository.Add(usuario);
        }

        public Usuario Update(int id, UsuarioRequest request)
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
            return _usuarioRepository.Update(existing);
        }

        public void Delete(int id)
        {
            _usuarioRepository.Delete(id);
        }

    }
}