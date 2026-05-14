using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> GetAll();
        Usuario GetById(int id);
        Usuario Add(Usuario entity);
        Usuario Update(Usuario entity);
        void Delete(int id);
    }
}
