namespace Application.Models.Requests
{
    public class GastoRequest
    {
        public int ViajeId { get; set; }
        public int ParticipanteId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }
}