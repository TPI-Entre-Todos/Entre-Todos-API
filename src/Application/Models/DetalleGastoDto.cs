namespace Application.Models
{
    public class DetalleGastoDto
    {
        public int Id { get; set; }
        public int GastoId { get; set; }
        public int ParticipanteId { get; set; }
        public decimal MontoIndividual { get; set; }
    }
}