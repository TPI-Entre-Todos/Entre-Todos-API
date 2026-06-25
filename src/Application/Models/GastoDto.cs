using Domain.Entities;
using Domain.Enums;

namespace Application.Models
{
    public class GastoDto
    {
        public int Id { get; set; }
        public int ViajeId { get; set; }
        public int ParticipanteId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public TipoDivision TipoDivision { get; set; }
        public string? Categoria { get; set; }
        public string? Comprobante { get; set; }
        public ICollection<DetalleGastoDto> Detalles { get; set; } = [];

        public static GastoDto Create(Gasto gasto)
        {
            return new GastoDto
            {
                Id = gasto.Id,
                ViajeId = gasto.ViajeId,
                ParticipanteId = gasto.ParticipanteId,
                Descripcion = gasto.Descripcion,
                Monto = gasto.Monto,
                Fecha = gasto.Fecha.ToString("dd/MM/yyyy HH:mm:ss"),
                TipoDivision = gasto.TipoDivision,
                Categoria = gasto.Categoria,
                Comprobante = gasto.Comprobante,
                Detalles = gasto.DetallesGasto != null
                    ? gasto.DetallesGasto.Select(DetalleGastoDto.Create).ToList()
                    : []
            };
        }

        public static List<GastoDto> CreateList(List<Gasto> gastos)
        {
            return gastos.Select(Create).ToList();
        }
    }
}