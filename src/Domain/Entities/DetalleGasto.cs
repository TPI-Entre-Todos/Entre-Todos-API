namespace Domain.Entities
{
    public class DetalleGasto
    {
        public int Id { get; set; }
        public int GastoId { get; set; }
        public int ParticipanteId { get; set; } // El participante que debe esta parte del gasto

        public decimal MontoDebe { get; set; }     // Lo que le corresponde pagar según la división
        public decimal MontoPagado { get; set; }   // Lo que ya abonó (vía Pago)
        public decimal SaldoPendiente => MontoDebe - MontoPagado; // Calculado, no persistido

        // Propiedades de navegación
        public Gasto? Gasto { get; set; }
        public ParticipanteViaje? Participante { get; set; }
        public DetalleGasto(int participanteId, decimal montoDebe, decimal montoPagado)
        {
            ParticipanteId = participanteId;
            MontoDebe = montoDebe;
            MontoPagado = montoPagado;
        }
    }
}
