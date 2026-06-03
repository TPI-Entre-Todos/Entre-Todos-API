using System;

namespace Domain.Entities
{
    public class Pago
    {
        public int Id { get; set; }
        public int ParticipanteId { get; set; }
        public int ViajeId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Metodo { get; set; }
        public string Comprobante { get; set; }

        // Navigation properties
        public ParticipanteViaje Participante { get; set; }
        public Viaje Viaje { get; set; }

        public Pago(int participanteId, int viajeId, decimal monto, string metodo, string comprobante)
        {
            ParticipanteId = participanteId;
            ViajeId = viajeId;
            Monto = monto;
            Fecha = DateTime.Now;
            Metodo = metodo;
            Comprobante = comprobante;
        }

        public Pago() { }
    }
}
