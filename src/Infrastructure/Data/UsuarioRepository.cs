using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{

    public UsuarioRepository(ApplicationContext context) : base(context)
    {
    }


    public Usuario GetUserByEmail(string email)
    {
        return _context.Usuarios.FirstOrDefault(u => u.Email == email);
    }

    public Usuario? GetByCognitoSub(string cognitoSub)
    {
        return _context.Usuarios.FirstOrDefault(u => u.CognitoSub == cognitoSub);
    }

}