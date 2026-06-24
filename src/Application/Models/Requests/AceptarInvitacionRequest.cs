namespace Application.Models.Requests;

public class AceptarInvitacionRequest
{
    public string Token { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
}
