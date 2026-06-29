using Domain.Entities;

namespace Application.Models;

public class PagoDto
{
    public int Id { get; set; }
    public int ParticipanteId { get; set; }
    public int DestinatarioId { get; set; }
    public int ViajeId { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string Metodo { get; set; }
    public string Comprobante { get; set; }
    public List<DetalleGastoPagadoDto> DetallesPagados { get; set; } = [];

    public static PagoDto Create(Pago pago)
    {
        if (pago == null)
            return null;

        return new PagoDto
        {
            Id = pago.Id,
            ParticipanteId = pago.RemitenteId,
            DestinatarioId = pago.DestinatarioId,
            ViajeId = pago.ViajeId,
            Monto = pago.Monto,
            Fecha = pago.Fecha,
            Metodo = pago.Metodo,
            Comprobante = pago.Comprobante,
            DetallesPagados = pago.DetallesPagados
                .Select(d => new DetalleGastoPagadoDto
                {
                    DetalleGastoId = d.Id,
                    GastoId = d.GastoId,
                    ParticipanteId = d.ParticipanteId,
                    MontoDebe = d.MontoDebe,
                    MontoPagado = d.MontoPagado,
                    SaldoPendiente = d.SaldoPendiente
                })
                .ToList()
        };
    }

    public static List<PagoDto> CreateList(List<Pago> pagos)
    {
        return pagos.Select(Create).ToList();
    }
}

public class DetalleGastoPagadoDto
{
    public int DetalleGastoId { get; set; }
    public int GastoId { get; set; }
    public int ParticipanteId { get; set; }
    public decimal MontoDebe { get; set; }
    public decimal MontoPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
}
