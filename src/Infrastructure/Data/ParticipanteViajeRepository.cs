using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        public async Task<List<ParticipanteViaje>> GetByViajeIdAsync(int viajeId)
        {
            return await _context.ParticipantesViaje
                .Include(pv => pv.Usuario) // Trae los datos del Usuario asociado
                .Where(pv => pv.ViajeId == viajeId)
                .ToListAsync();
        }

        public List<ParticipanteViaje> GetByViajeId(int viajeId)
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .Where(pv => pv.ViajeId == viajeId)
                .ToList();
        }

        // Busca si un usuario ya pertenece a un viaje (sirve para evitar duplicados)
        public async Task<ParticipanteViaje?> GetByIdsAsync(int usuarioId, int viajeId)
        {
            return await _context.ParticipantesViaje
                .FirstOrDefaultAsync(pv => pv.UsuarioId == usuarioId && pv.ViajeId == viajeId);
        }

        public ParticipanteViaje? GetByIds(int usuarioId, int viajeId)
        {
            return _context.ParticipantesViaje
                .FirstOrDefault(pv => pv.UsuarioId == usuarioId && pv.ViajeId == viajeId);
        }

        // Busca un participante por su ID único
        public async Task<ParticipanteViaje?> GetByIdAsync(int id)
        {
            return await _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .FirstOrDefaultAsync(pv => pv.Id == id);
        }

        public ParticipanteViaje? GetById(int id)
        {
            return _context.ParticipantesViaje
                .Include(pv => pv.Usuario)
                .FirstOrDefault(pv => pv.Id == id);
        }

        // Guarda un nuevo participante en la base de datos (Alta)
        public async Task<ParticipanteViaje> AddAsync(ParticipanteViaje entity)
        {
            await _context.ParticipantesViaje.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public ParticipanteViaje Add(ParticipanteViaje entity)
        {
            _context.ParticipantesViaje.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        // Actualiza los datos del participante (Modificación, ej: aceptar invitación o cambiar saldo)
        public async Task UpdateAsync(ParticipanteViaje entity)
        {
            _context.ParticipantesViaje.Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Update(ParticipanteViaje entity)
        {
            _context.ParticipantesViaje.Update(entity);
            _context.SaveChanges();
        }

        // Borra un participante de la base de datos (Baja)
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.ParticipantesViaje.Remove(entity);
                await _context.SaveChangesAsync();
            }
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