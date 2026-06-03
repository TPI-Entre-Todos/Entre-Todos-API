using System;

namespace Application.Models
{
    public class GastoDto
    {
        public int Id { get; set; }
        public int ViajeId { get; set; }
        public int ParticipanteId { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
    }
}