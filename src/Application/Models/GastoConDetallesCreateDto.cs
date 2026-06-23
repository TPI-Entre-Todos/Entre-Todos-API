using System.Collections.Generic;

namespace Application.Models
{
    public class GastoConDetallesCreateDto
    {
        public int ViajeId { get; set; }
        public int PagadorParticipanteId { get; set; } // Quién puso la plata
        public string Descripcion { get; set; }
        public decimal MontoTotal { get; set; }
        public List<DetalleGastoCreateDto> Divisiones { get; set; } // La lista de quiénes dividen
    }
}