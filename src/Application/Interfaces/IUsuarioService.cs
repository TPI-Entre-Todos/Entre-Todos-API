
using Application.Models.Requests;
using Domain.Entities;


namespace Application.Interfaces
{
    public interface IUsuarioService
    {
        List<Usuario> GetAll();
        Usuario GetById(int id);
        Usuario Add(UsuarioRequest request);
        Usuario Update(int id, UsuarioRequest request);
        void Delete(int id);
    }
}