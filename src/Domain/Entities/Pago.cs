using System;

namespace Domain.Entities
{
    public class Pago
    {
        public int Id { get; set; }
        public int ViajeId { get; set; }
        
        public int RemitenteId { get; set; } // El que transfiere
        public int DestinatarioId { get; set; } // El que recibe
        
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Metodo { get; set; }
        public string Comprobante { get; set; }

        // Propiedades de navegación
        public Viaje? Viaje { get; set; }
        public ParticipanteViaje? Remitente { get; set; }
        public ParticipanteViaje? Destinatario { get; set; }
        public ICollection<DetalleGasto> DetallesPagados { get; set; } = [];

        public Pago(int remitenteId, int destinatarioId, int viajeId, decimal monto, string metodo, string comprobante)
        {
            RemitenteId = remitenteId;
            DestinatarioId = destinatarioId;
            ViajeId = viajeId;
            Monto = monto;
            Fecha = DateTime.Now;
            Metodo = metodo;
            Comprobante = comprobante;
        }

        public Pago() { }
    }
}