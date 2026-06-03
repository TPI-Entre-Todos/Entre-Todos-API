namespace Application.Models.Requests;

public class InvitacionRequest
{
    public int ViajeId { get; set; }
    public int UsuarioInvitadorId { get; set; }
    public string EmailInvitado { get; set; } = string.Empty;
    public DateTime FechaExpiracion { get; set; }
}
