using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class InvitacionRepository : GenericRepository<Invitacion>, IInvitacionRepository
{
    public InvitacionRepository(ApplicationContext context) : base(context)
    {
    }

    public override List<Invitacion> GetAll()
    {
        return _context.Invitaciones
            .Include(i => i.Viaje)
            .Include(i => i.UsuarioInvitador)
            .ToList();
    }

    public override Invitacion GetById(int id)
    {
        return _context.Invitaciones
            .Include(i => i.Viaje)
            .Include(i => i.UsuarioInvitador)
            .FirstOrDefault(i => i.Id == id);
    }

    public Invitacion? GetByToken(string token)
    {
        return _context.Invitaciones
            .Include(i => i.Viaje)
            .Include(i => i.UsuarioInvitador)
            .FirstOrDefault(i => i.Token == token);
    }

    public override Invitacion Update(Invitacion entity)
    {
        _context.Invitaciones.Update(entity);
        _context.SaveChanges();
        return entity;
    }
}
