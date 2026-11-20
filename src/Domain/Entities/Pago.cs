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

        // Key de S3 del archivo del comprobante ("pagos/<guid>.pdf"), en un bucket privado.
        public string? ComprobanteKey { get; set; }

        // Vincula pagos que son una sola operación (ej. una compensación A→B y B→A).
        // Al eliminar uno se eliminan todos los del grupo.
        public Guid? GrupoPagoId { get; set; }

        // Propiedades de navegación
        public Viaje? Viaje { get; set; }
        public ParticipanteViaje? Remitente { get; set; }
        public ParticipanteViaje? Destinatario { get; set; }
        public ICollection<PagoDetalle> DetallesPagados { get; set; } = [];

        public Pago(int remitenteId, int destinatarioId, int viajeId, decimal monto, string metodo)
        {
            RemitenteId = remitenteId;
            DestinatarioId = destinatarioId;
            ViajeId = viajeId;
            Monto = monto;
            Fecha = DateTime.Now;
            Metodo = metodo;
        }

        public Pago() { }
    }
}