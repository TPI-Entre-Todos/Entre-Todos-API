using Domain.Entities;
using Domain.Enums;

namespace Application.Models;

public class InvitacionDto
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public EstadoInvitacion Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaExpiracion { get; set; }
    public int ViajeId { get; set; }
    public int UsuarioInvitadorId { get; set; }
    public string EmailInvitado { get; set; } = string.Empty;
    public DateTime? FechaRespuesta { get; set; }

    public static InvitacionDto Create(Invitacion invitacion)
    {
        return new InvitacionDto
        {
            Id = invitacion.Id,
            Token = invitacion.Token,
            Estado = invitacion.Estado,
            FechaCreacion = invitacion.FechaCreacion,
            FechaExpiracion = invitacion.FechaExpiracion,
            ViajeId = invitacion.ViajeId,
            UsuarioInvitadorId = invitacion.UsuarioInvitadorId,
            EmailInvitado = invitacion.EmailInvitado,
            FechaRespuesta = invitacion.FechaRespuesta
        };
    }

    public static List<InvitacionDto> CreateList(List<Invitacion> invitaciones)
    {
        var dtos = new List<InvitacionDto>();
        foreach (var invitacion in invitaciones)
        {
            dtos.Add(Create(invitacion));
        }
        return dtos;
    }
}
