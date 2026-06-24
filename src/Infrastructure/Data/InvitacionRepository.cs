using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class InvitacionRepository : IInvitacionRepository
{
    protected readonly ApplicationContext _context;

    public InvitacionRepository(ApplicationContext dbContext)
    {
        _context = dbContext;
    }

    public List<Invitacion> GetAll()
    {
        return _context.Invitaciones
            .Include(i => i.Viaje)
            .Include(i => i.UsuarioInvitador)
            .ToList();
    }

    public Invitacion? GetById(int id)
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

    public Invitacion Add(Invitacion entity)
    {
        _context.Invitaciones.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public void Update(Invitacion entity)
    {
        _context.Invitaciones.Update(entity);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var invitacion = _context.Invitaciones.FirstOrDefault(i => i.Id == id);
        if (invitacion != null)
        {
            _context.Invitaciones.Remove(invitacion);
            _context.SaveChanges();
        }
    }
}