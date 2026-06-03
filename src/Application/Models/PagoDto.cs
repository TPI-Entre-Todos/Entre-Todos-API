using Domain.Entities;

namespace Application.Models;

public class PagoDto
{
    public int Id { get; set; }
    public int ParticipanteId { get; set; }
    public int ViajeId { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string Metodo { get; set; }
    public string Comprobante { get; set; }

    public static PagoDto Create(Pago pago)
    {
        if (pago == null)
            return null;

        var dto = new PagoDto
        {
            Id = pago.Id,
            ParticipanteId = pago.ParticipanteId,
            ViajeId = pago.ViajeId,
            Monto = pago.Monto,
            Fecha = pago.Fecha,
            Metodo = pago.Metodo,
            Comprobante = pago.Comprobante
        };
        return dto;
    }

    public static List<PagoDto> CreateList(List<Pago> pagos)
    {
        var dtos = new List<PagoDto>();
        foreach (var pago in pagos)
        {
            dtos.Add(Create(pago));
        }
        return dtos;
    }
}
