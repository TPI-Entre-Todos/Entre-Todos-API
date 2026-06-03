using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class PagoRepository : IPagoRepository
{
    protected readonly ApplicationContext _context;

    public PagoRepository(ApplicationContext dbContext)
    {
        _context = dbContext;
    }

    public List<Pago> GetAll()
    {
        return _context.Pagos
            .Include(p => p.Participante)
            .Include(p => p.Viaje)
            .ToList();
    }

    public Pago GetById(int id)
    {
        return _context.Pagos
            .Include(p => p.Participante)
            .Include(p => p.Viaje)
            .FirstOrDefault(p => p.Id == id);
    }

    public Pago Add(Pago entity)
    {
        _context.Pagos.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public Pago Update(Pago entity)
    {
        var existing = _context.Pagos.FirstOrDefault(p => p.Id == entity.Id);
        if (existing == null)
        {
            return null;
        }

        existing.ParticipanteId = entity.ParticipanteId;
        existing.ViajeId = entity.ViajeId;
        existing.Monto = entity.Monto;
        existing.Fecha = entity.Fecha;
        existing.Metodo = entity.Metodo;
        existing.Comprobante = entity.Comprobante;

        _context.SaveChanges();
        return existing;
    }

    public void Delete(int id)
    {
        var pago = _context.Pagos.FirstOrDefault(p => p.Id == id);
        if (pago != null)
        {
            _context.Pagos.Remove(pago);
            _context.SaveChanges();
        }
    }

    public List<Pago> GetByViajeId(int viajeId)
    {
        return _context.Pagos
            .Include(p => p.Participante)
            .Include(p => p.Viaje)
            .Where(p => p.ViajeId == viajeId)
            .ToList();
    }

    public List<Pago> GetByParticipanteId(int participanteId)
    {
        return _context.Pagos
            .Include(p => p.Participante)
            .Include(p => p.Viaje)
            .Where(p => p.ParticipanteId == participanteId)
            .ToList();
    }
}
