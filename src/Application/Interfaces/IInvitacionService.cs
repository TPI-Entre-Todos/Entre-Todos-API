using Application.Models;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IInvitacionService
    {
        /// <summary>Todas las invitaciones (solo Admin).</summary>
        List<InvitacionDto> GetAll();

        /// <summary>Invitaciones enviadas al email del usuario autenticado.</summary>
        List<InvitacionDto> GetMias(int usuarioId);

        /// <summary>Invitaciones de un viaje. Solo organizador o Admin. Sin token.</summary>
        List<InvitacionDto> GetPorViaje(int viajeId, int usuarioId, bool esAdmin);

        InvitacionDto? GetById(int id, int usuarioId, bool esAdmin);

        /// <summary>Crea la invitación y envía el email. Solo organizador del viaje o Admin.</summary>
        InvitacionDto Add(InvitacionRequest request, int usuarioId, bool esAdmin);

        InvitacionDto AceptarInvitacion(string token, int usuarioId);
        InvitacionDto RechazarInvitacion(string token, int usuarioId);

        /// <summary>Cancela/elimina. Solo Admin, organizador del viaje o quien invitó.</summary>
        void Delete(int id, int usuarioId, bool esAdmin);
    }
}
