using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class GastoRepository : GenericRepository<Gasto>, IGastoRepository
    {
        public GastoRepository(ApplicationContext context) : base(context)
        {
        }

        public override Gasto GetById(int id)
        {
            return _context.Gastos
                .Include(g => g.DetallesGasto)
                .FirstOrDefault(g => g.Id == id);
        }

        public override List<Gasto> GetAll()
        {
            return _context.Gastos
                .Include(g => g.DetallesGasto)
                .ToList();
        }

        public List<Gasto> GetByViajeId(int viajeId)
        {
            return _context.Gastos
                .Include(g => g.DetallesGasto)
                .Where(g => g.ViajeId == viajeId)
                .ToList();
        }

        public Gasto AddWithDetalles(Gasto gasto, Dictionary<int, decimal> saldoChanges)
        {
            _context.Gastos.Add(gasto);
            AplicarCambiosSaldo(saldoChanges);
            _context.SaveChanges();
            return gasto;
        }

        public Gasto UpdateWithDetalles(Gasto gasto, Dictionary<int, decimal> saldoChanges)
        {
            _context.Gastos.Update(gasto);
            AplicarCambiosSaldo(saldoChanges);
            _context.SaveChanges();
            return gasto;
        }

        public void DeleteWithSaldoReversal(int id)
        {
            var entity = GetById(id);
            if (entity == null) return;

            var saldoReversal = new Dictionary<int, decimal>
            {
                [entity.ParticipanteId] = -entity.Monto
            };

            foreach (var detalle in entity.DetallesGasto)
            {
                if (saldoReversal.ContainsKey(detalle.ParticipanteId))
                    saldoReversal[detalle.ParticipanteId] += detalle.MontoDebe;
                else
                    saldoReversal[detalle.ParticipanteId] = detalle.MontoDebe;
            }

            _context.Gastos.Remove(entity);
            AplicarCambiosSaldo(saldoReversal);
            _context.SaveChanges();
        }

        private void AplicarCambiosSaldo(Dictionary<int, decimal> saldoChanges)
        {
            foreach (var (participanteId, delta) in saldoChanges)
            {
                var participante = _context.ParticipantesViaje.Find(participanteId);
                if (participante != null)
                    participante.SaldoTotal += delta;
            }
        }
    }
}
