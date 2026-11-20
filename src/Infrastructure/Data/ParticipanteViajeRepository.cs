using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ParticipanteViajeRepository : GenericRepository<ParticipanteViaje>, IParticipanteViajeRepository
    {
        public ParticipanteViajeRepository(ApplicationContext context) : base(context)
        {
        }

        public List<ParticipanteViaje> GetByViajeId(int viajeId)
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .Where(pv => pv.ViajeId == viajeId)
                .ToList();
        }

        public List<ParticipanteViaje> ObtenerTodos()
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .ToList();
        }

        public List<ParticipanteViaje> GetByUsuarioId(int usuarioId)
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .Where(pv => pv.UsuarioId == usuarioId)
                .ToList();
        }

        public ParticipanteViaje? GetByIds(int usuarioId, int viajeId)
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .FirstOrDefault(pv => pv.UsuarioId == usuarioId && pv.ViajeId == viajeId);
        }

        // Son las mismas tablas que apuntan al participante con FK Restrict: si alguna tiene
        // filas, la base no deja borrarlo.
        public bool TieneMovimientos(int participanteId)
        {
            return _context.Gastos.Any(g => g.ParticipanteId == participanteId)
                || _context.DetallesGasto.Any(d => d.ParticipanteId == participanteId)
                || _context.Pagos.Any(p => p.RemitenteId == participanteId || p.DestinatarioId == participanteId);
        }

        public override ParticipanteViaje GetById(int id)
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .FirstOrDefault(pv => pv.Id == id);
        }
    }
}
