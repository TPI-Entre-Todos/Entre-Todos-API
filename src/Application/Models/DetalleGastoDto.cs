using Domain.Entities;

namespace Application.Models
{
    public class DetalleGastoDto
    {
        public int Id { get; set; }
        public int GastoId { get; set; }
        public int ParticipanteId { get; set; }
        public decimal MontoDebe { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }

        public static DetalleGastoDto Create(DetalleGasto detalle)
        {
            return new DetalleGastoDto
            {
                Id = detalle.Id,
                GastoId = detalle.GastoId,
                ParticipanteId = detalle.ParticipanteId,
                MontoDebe = detalle.MontoDebe,
                MontoPagado = detalle.MontoPagado,
                SaldoPendiente = detalle.SaldoPendiente
            };
        }

        public static List<DetalleGastoDto> CreateList(List<DetalleGasto> detalles)
        {
            return detalles.Select(Create).ToList();
        }
    }
}
