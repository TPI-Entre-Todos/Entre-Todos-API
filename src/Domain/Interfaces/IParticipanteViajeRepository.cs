using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Interfaces
{
    public interface IParticipanteViajeRepository
    {
        Task<List<ParticipanteViaje>> GetByViajeIdAsync(int viajeId);
        Task<ParticipanteViaje> GetByIdsAsync(int usuarioId, int viajeId);
        Task<ParticipanteViaje> GetByIdAsync(int id);
        Task<ParticipanteViaje> AddAsync(ParticipanteViaje entity);
        Task UpdateAsync(ParticipanteViaje entity);
        Task DeleteAsync(int id);
    }
}