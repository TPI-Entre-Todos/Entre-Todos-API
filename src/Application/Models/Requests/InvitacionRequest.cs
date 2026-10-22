namespace Application.Models.Requests;

/// <summary>
/// Quien invita se toma del token (no del body), para que no se pueda invitar "en nombre" de otro.
/// </summary>
public class InvitacionRequest
{
    public int ViajeId { get; set; }
    public string EmailInvitado { get; set; } = string.Empty;
    public DateTime FechaExpiracion { get; set; }
}
