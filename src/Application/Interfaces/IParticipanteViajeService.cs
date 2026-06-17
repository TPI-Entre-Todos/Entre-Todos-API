using Application.Models.Requests;
using Domain.Entities;
using Application.Models;

namespace Application.Interfaces
{
    public interface IParticipanteViajeService
    {
        ParticipanteViajeDto RegistrarParticipante(ParticipanteViajeCreateRequest dto);
        List<ParticipanteViajeDto> ObtenerPorViaje(int viajeId, int usuarioId);

        List<ParticipanteViajeDto> ObtenerPorViajeAdmin(int viajeId);
        List<ParticipanteViajeDto> ObtenerTodos(int usuarioId, bool esAdmin);
        void EliminarParticipante(int id);

    }
}