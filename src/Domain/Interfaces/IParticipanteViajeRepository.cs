using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Interfaces
{
    public interface IParticipanteViajeRepository : IGenericRepository<ParticipanteViaje>
    {
        List<ParticipanteViaje> GetByViajeId(int viajeId);
        List<ParticipanteViaje> GetByUsuarioId(int usuarioId);
        ParticipanteViaje? GetByIds(int usuarioId, int viajeId);
        List<ParticipanteViaje> ObtenerTodos();

        // True si el participante pagó algún gasto, tiene parte en alguno, o envió/recibió pagos.
        bool TieneMovimientos(int participanteId);
    }
}
