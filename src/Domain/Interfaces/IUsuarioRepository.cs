using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Usuario GetUserByEmail(string email);
        Usuario? GetByCognitoSub(string cognitoSub);
    }
}
