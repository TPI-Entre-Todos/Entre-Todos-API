using Application.Models.Requests;
using Domain.Entities;
using Application.Models;

namespace Application.Interfaces
{
    public interface IParticipanteViajeService
    {
        Task<ParticipanteViajeDto> RegistrarParticipanteAsync(ParticipanteViajeCreateDto dto);
        Task<List<ParticipanteViajeDto>> ObtenerPorViajeAsync(int viajeId);
        Task ResponderInvitacionAsync(int id, string nuevoEstado);
        Task EliminarParticipanteAsync(int id);

    }
}