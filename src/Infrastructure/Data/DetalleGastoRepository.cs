using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Data
{
    public class DetalleGastoRepository : GenericRepository<DetalleGasto>, IDetalleGastoRepository
    {
        public DetalleGastoRepository(ApplicationContext context) : base(context)
        {
        }

        public List<DetalleGasto> GetByGastoId(int gastoId)
        {
            return _context.DetallesGasto
                .Where(d => d.GastoId == gastoId)
                .ToList();
        }

        public List<DetalleGasto> GetByParticipanteId(int participanteId)
        {
            return _context.DetallesGasto
                .Where(d => d.ParticipanteId == participanteId)
                .ToList();
        }

        public void AddRange(List<DetalleGasto> entities)
        {
            _context.DetallesGasto.AddRange(entities);
            _context.SaveChanges();
        }

        public DetalleGasto Update(DetalleGasto entity)
        {
            _context.DetallesGasto.Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.DetallesGasto.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}
