using Application.Models.Requests;
using Domain.Entities;
using Application.Models;

namespace Application.Interfaces
{
    public interface IParticipanteViajeService
    {
        ParticipanteViajeDto RegistrarParticipante(ParticipanteViajeCreateRequest dto);
        List<ParticipanteViajeDto> ObtenerPorViaje(int viajeId);
        List<ParticipanteViajeDto> ObtenerTodos();
        void EliminarParticipante(int id);

    }
}