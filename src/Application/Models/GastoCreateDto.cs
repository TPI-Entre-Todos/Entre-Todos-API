namespace Application.Models
{
    public class GastoCreateDto
    {
        public int ViajeId { get; set; }
        public int ParticipanteId { get; set; } // El ID del participante que puso la plata
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
    }
}