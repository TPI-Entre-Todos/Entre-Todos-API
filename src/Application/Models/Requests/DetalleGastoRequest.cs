namespace Application.Models.Requests
{
    public class DetalleGastoRequest
    {
        public int Id { get; set; }
        public int ParticipanteId { get; set; }
        public decimal MontoIndividual { get; set; }
    }
}