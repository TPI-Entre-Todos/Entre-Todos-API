namespace Application.Models.Requests;

public class PagoRequest
{
    public int ParticipanteId { get; set; }
    public int DestinatarioId { get; set; }
    public int ViajeId { get; set; }
    public decimal? Monto { get; set; }
    public string Metodo { get; set; }
    public string Comprobante { get; set; }
}
