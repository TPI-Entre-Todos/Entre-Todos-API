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
    public string? ViajeNombre { get; set; }
    public int UsuarioInvitadorId { get; set; }
    public string? UsuarioInvitadorNombre { get; set; }
    public string EmailInvitado { get; set; } = string.Empty;
    public DateTime? FechaRespuesta { get; set; }

    public static InvitacionDto Create(Invitacion invitacion)
    {
        // Una invitación pendiente que ya venció se informa como Expirada.
        var estado = invitacion.Estado == EstadoInvitacion.Pendiente && invitacion.FechaExpiracion < DateTime.UtcNow
            ? EstadoInvitacion.Expirada
            : invitacion.Estado;

        return new InvitacionDto
        {
            Id = invitacion.Id,
            Token = invitacion.Token,
            Estado = estado,
            FechaCreacion = invitacion.FechaCreacion,
            FechaExpiracion = invitacion.FechaExpiracion,
            ViajeId = invitacion.ViajeId,
            ViajeNombre = invitacion.Viaje?.Nombre,
            UsuarioInvitadorId = invitacion.UsuarioInvitadorId,
            UsuarioInvitadorNombre = invitacion.UsuarioInvitador?.Nombre,
            EmailInvitado = invitacion.EmailInvitado,
            FechaRespuesta = invitacion.FechaRespuesta
        };
    }

    /// <summary>
    /// El token es la "llave" para aceptar: solo lo ve el invitado (o un Admin), nunca
    /// quien lista las invitaciones de un viaje.
    /// </summary>
    public InvitacionDto SinToken()
    {
        Token = string.Empty;
        return this;
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
