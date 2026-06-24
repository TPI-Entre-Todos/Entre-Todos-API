namespace Application.Models
{
    public class GastoRequest
    {
        public int ViajeId { get; set; }
        public int ParticipanteId { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
    }
}