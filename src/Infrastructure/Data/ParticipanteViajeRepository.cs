using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Data
{
    public class ParticipanteViajeRepository : IParticipanteViajeRepository
    {
        private readonly ApplicationContext _context;

        // El constructor recibe el ApplicationContext que ya tenés creado
        public ParticipanteViajeRepository(ApplicationContext context)
        {
            _context = context;
        }

        // Obtiene todos los participantes de un viaje en particular
        public List<ParticipanteViaje> GetByViajeId(int viajeId)
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .Where(pv => pv.ViajeId == viajeId)
                .ToList();
        }

        // Obtiene todos los participantes
        public List<ParticipanteViaje> ObtenerTodos()
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .ToList();
        }

        // Obtiene los participantes asociados a un usuario
        public List<ParticipanteViaje> GetByUsuarioId(int usuarioId)
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .Where(pv => pv.UsuarioId == usuarioId)
                .ToList();
        }

        // Busca si un usuario ya pertenece a un viaje (sirve para evitar duplicados)
        public ParticipanteViaje? GetByIds(int usuarioId, int viajeId)
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .FirstOrDefault(pv => pv.UsuarioId == usuarioId && pv.ViajeId == viajeId);
        }
        // Busca un participante por su ID único
        public ParticipanteViaje? GetById(int id)
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .FirstOrDefault(pv => pv.Id == id);
        }

        // Guarda un nuevo participante en la base de datos (Alta)

        public ParticipanteViaje Add(ParticipanteViaje entity)
        {
            _context.ParticipantesViaje.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        // Actualiza los datos del participante (Modificación, ej: aceptar invitación o cambiar saldo)

        public ParticipanteViaje Update(ParticipanteViaje entity)
        {
            _context.ParticipantesViaje.Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.ParticipantesViaje.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}