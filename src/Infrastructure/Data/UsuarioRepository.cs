using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class UsuarioRepository : IUsuarioRepository
{
    protected readonly ApplicationContext _context;

    public UsuarioRepository(ApplicationContext dbContext)
    {
        _context = dbContext;
    }

    public List<Usuario> GetAll()
    {
        return _context.Usuarios.ToList();
    }

    public Usuario GetById(int id)
    {
        return _context.Usuarios.FirstOrDefault(u => u.Id == id);
    }

    public Usuario Add(Usuario entity)
    {
        _context.Usuarios.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public Usuario Update(Usuario entity)
    {
        var existing = _context.Usuarios.FirstOrDefault(u => u.Id == entity.Id);
        if (existing == null)
        {
            return null;
        }

        existing.Nombre = entity.Nombre;
        existing.Email = entity.Email;
        existing.Password = entity.Password;
        existing.Rol = entity.Rol;

        _context.SaveChanges();
        return existing;
    }

    public void Delete(int id)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();
        }
    }

}