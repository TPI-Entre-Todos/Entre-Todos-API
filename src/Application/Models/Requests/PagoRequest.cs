using System.ComponentModel.DataAnnotations;

namespace Application.Models.Requests
{
    // Base compartida
    public class PagoBaseRequest
    {
        public int ParticipanteId { get; set; }
        public int DestinatarioId { get; set; }
        public int ViajeId { get; set; }

        [Required]
        public decimal? Monto { get; set; }

        [Required]
        public string Metodo { get; set; } = string.Empty;

        public string Comprobante { get; set; } = string.Empty;
    }

    // Para pagar un solo DetalleGasto
    public class PagoSimpleRequest : PagoBaseRequest
    {
        public int DetalleGastoId { get; set; }
    }

    // Para pagar múltiples DetalleGastos a la vez
    public class PagoMultipleRequest : PagoBaseRequest
    {
        public List<PagoDetalleGastoItem> DetallesPagados { get; set; } = [];
    }

    // Mantener PagoRequest para compatibilidad con Update
    public class PagoRequest : PagoBaseRequest
    {
    }

    public class PagoDetalleGastoItem
    {
        public int DetalleGastoId { get; set; }
        public decimal Monto { get; set; }
    }
}
