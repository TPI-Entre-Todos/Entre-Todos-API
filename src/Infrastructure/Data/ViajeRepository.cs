using Domain;
using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ViajeRepository : IViajeRepository
{
    protected readonly ApplicationContext _context;
    public ViajeRepository(ApplicationContext dbContext)
    {
        _context = dbContext;
    }

    public Viaje GetById(int id)
    {
        return _context.Viajes.FirstOrDefault(v => v.Id == id);
        
    }

     public Viaje Add(Viaje entity)
    {
        _context.Viajes.Add(entity);

        _context.SaveChanges();

        return entity;
    }

    public void Delete(int id)
    {
        _context.Viajes.FirstOrDefault(v => v.Id == id);

        _context.SaveChanges();
    }
}