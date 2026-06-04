using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Interfaces
{
    public interface IParticipanteViajeRepository
    {
        Task<List<ParticipanteViaje>> GetByViajeIdAsync(int viajeId);
        Task<ParticipanteViaje?> GetByIdsAsync(int usuarioId, int viajeId);
        Task<ParticipanteViaje?> GetByIdAsync(int id);
        Task<ParticipanteViaje> AddAsync(ParticipanteViaje entity);
        Task UpdateAsync(ParticipanteViaje entity);
        Task DeleteAsync(int id);

        List<ParticipanteViaje> GetByViajeId(int viajeId);
        ParticipanteViaje? GetByIds(int usuarioId, int viajeId);
        ParticipanteViaje? GetById(int id);
        ParticipanteViaje Add(ParticipanteViaje entity);
        void Update(ParticipanteViaje entity);
        void Delete(int id);
    }
}