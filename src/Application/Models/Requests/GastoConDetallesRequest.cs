using Domain.Enums;

namespace Application.Models.Requests
{
    public class GastoConDetallesRequest
    {
        public int ViajeId { get; set; }
        public int ParticipanteId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public TipoDivision TipoDivision { get; set; } = TipoDivision.Igualitario;
        public string? Categoria { get; set; }
        public string? Comprobante { get; set; }
        public List<DetalleGastoItemRequest> Detalles { get; set; } = [];
    }
}
