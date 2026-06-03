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

        // Propiedades de navegación (por si EF las necesita)
        public Viaje Viaje { get; set; }
        public ParticipanteViaje Participante { get; set; }
    }
}