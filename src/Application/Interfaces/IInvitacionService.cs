using Application.Models;
using Application.Models.Requests;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IInvitacionService
    {
        List<InvitacionDto> GetAll();
        InvitacionDto? GetById(int id);
        InvitacionDto Add(InvitacionRequest request);
        InvitacionDto AceptarInvitacion(string token, int usuarioId);
        InvitacionDto RechazarInvitacion(string token);
        void Delete(int id);
    }
}
