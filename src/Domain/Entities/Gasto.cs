using Domain.Enums;

namespace Domain.Entities
{
    public class Gasto
    {
        public int Id { get; set; }
        public int ViajeId { get; set; }
        public int ParticipanteId { get; set; } // Quién pagó el gasto
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public TipoDivision TipoDivision { get; set; }
        public string? Categoria { get; set; }
        public string? Comprobante { get; set; }

        public Viaje? Viaje { get; set; }
        public ParticipanteViaje? Participante { get; set; }
        public ICollection<DetalleGasto> DetallesGasto { get; set; } = [];

        public Gasto(int viajeId, int participanteId, string descripcion, decimal monto, TipoDivision tipoDivision, string? categoria, string? comprobante)
        {
            ViajeId = viajeId;
            ParticipanteId = participanteId;
            Descripcion = descripcion;
            Monto = monto;
            TipoDivision = tipoDivision;
            Fecha = DateTime.UtcNow;
            Categoria = categoria;
            Comprobante = comprobante;
        }

        public Gasto() { }
    }
}