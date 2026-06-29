using System.ComponentModel.DataAnnotations;

namespace Application.Models.Requests
{
    public class PagoRequest
    {
        [Required]
        public int? ParticipanteId { get; set; } 

        [Required]
        public int? DestinatarioId { get; set; } 

        [Required]
        public int? ViajeId { get; set; }

        [Required]
        public decimal? Monto { get; set; }

        [Required]
        public string Metodo { get; set; } = string.Empty;

        public string Comprobante { get; set; } = string.Empty;
    }
}