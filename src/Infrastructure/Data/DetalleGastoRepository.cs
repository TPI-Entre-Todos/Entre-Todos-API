using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DetalleGastoRepository : IDetalleGastoRepository
    {
        private readonly ApplicationContext _context;

        public DetalleGastoRepository(ApplicationContext context)
        {
            _context = context;
        }

        public DetalleGasto GetById(int id)
        {
            return _context.DetallesGasto.FirstOrDefault(d => d.Id == id);
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

        public DetalleGasto Add(DetalleGasto entity)
        {
            _context.DetallesGasto.Add(entity);
            _context.SaveChanges();
            return entity;
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