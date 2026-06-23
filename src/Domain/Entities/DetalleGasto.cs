namespace Domain.Entities
{
    public class DetalleGasto
    {
        public int Id { get; set; }
        public int GastoId { get; set; }
        public int ParticipanteId { get; set; } // El participante que "debe" esta parte
        public decimal MontoIndividual { get; set; } // Lo que le toca pagar a este participante

        // Propiedades de navegación
        public Gasto Gasto { get; set; }
        public ParticipanteViaje Participante { get; set; }
    }
}