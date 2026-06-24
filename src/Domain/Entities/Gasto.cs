using System;

namespace Domain.Entities
{
    public class Gasto
    {
        public int Id { get; set; }
        public int ViajeId { get; set; }
        public int ParticipanteId { get; set; } // Quién pagó el gasto
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }

        public Viaje Viaje { get; set; }
        public ParticipanteViaje Participante { get; set; }
        public ICollection<DetalleGasto> DetallesGasto { get; set; } = new List<DetalleGasto>();
        public Gasto(int viajeId, int participanteId, string descripcion, decimal monto)
        {
            ViajeId = viajeId;
            ParticipanteId = participanteId;
            Descripcion = descripcion;
            Monto = monto;
            Fecha = DateTime.Now;
        }

    }
}