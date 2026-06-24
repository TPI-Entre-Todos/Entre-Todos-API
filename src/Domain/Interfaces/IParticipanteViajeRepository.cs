using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Interfaces
{
    public interface IParticipanteViajeRepository
    {
        List<ParticipanteViaje> GetByViajeId(int viajeId);
        List<ParticipanteViaje> GetByUsuarioId(int usuarioId);
        ParticipanteViaje? GetByIds(int usuarioId, int viajeId);
        ParticipanteViaje? GetById(int id);
        ParticipanteViaje Add(ParticipanteViaje entity);
        void Update(ParticipanteViaje entity);
        List<ParticipanteViaje> ObtenerTodos();
        void Delete(int id);
    }
}