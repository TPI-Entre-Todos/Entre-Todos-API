using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class PagoRepository : GenericRepository<Pago>, IPagoRepository
{
    public PagoRepository(ApplicationContext context) : base(context)
    {
    }

    public override List<Pago> GetAll()
    {
        return _context.Pagos
            .Include(p => p.Remitente)
            .Include(p => p.Destinatario)
            .Include(p => p.Viaje)
            .ToList();
    }

    public override Pago GetById(int id)
    {
        return _context.Pagos
            .Include(p => p.Remitente)
            .Include(p => p.Destinatario)
            .Include(p => p.Viaje)
            .FirstOrDefault(p => p.Id == id);
    }

    public override Pago Update(Pago entity)
    {
        var existing = _context.Pagos.FirstOrDefault(p => p.Id == entity.Id);
        if (existing == null)
        {
            return null;
        }

        existing.RemitenteId = entity.RemitenteId;
        existing.DestinatarioId = entity.DestinatarioId;
        existing.ViajeId = entity.ViajeId;
        existing.Monto = entity.Monto;
        existing.Fecha = entity.Fecha;
        existing.Metodo = entity.Metodo;
        existing.Comprobante = entity.Comprobante;

        _context.SaveChanges();
        return existing;
    }

    public List<Pago> GetByViajeId(int viajeId)
    {
        return _context.Pagos
            .Include(p => p.Remitente)
            .Include(p => p.Destinatario)
            .Include(p => p.Viaje)
            .Where(p => p.ViajeId == viajeId)
            .ToList();
    }

    public List<Pago> GetByParticipanteId(int participanteId)
    {
        return _context.Pagos
            .Include(p => p.Remitente)
            .Include(p => p.Destinatario)
            .Include(p => p.Viaje)
            .Where(p => p.RemitenteId == participanteId)
            .ToList();
    }
}
