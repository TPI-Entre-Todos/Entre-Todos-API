namespace Domain.Entities
{
    // Cuánto de un Pago se aplicó a un DetalleGasto puntual.
    // Permite revertir exactamente lo que aportó ese pago, aunque el detalle tenga otros pagos parciales.
    public class PagoDetalle
    {
        public int Id { get; set; }
        public int PagoId { get; set; }
        public int DetalleGastoId { get; set; }
        public decimal Monto { get; set; }

        public Pago? Pago { get; set; }
        public DetalleGasto? DetalleGasto { get; set; }

        public PagoDetalle(int detalleGastoId, decimal monto)
        {
            DetalleGastoId = detalleGastoId;
            Monto = monto;
        }
    }
}
