namespace Application.Models
{
    public class SaldoDto
    {
        public int ParticipanteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal TotalPagadoBolsillo { get; set; } // Lo que puso en Gastos
        public decimal TotalConsumidoDebe { get; set; }  // Lo que debe en DetallesGasto
        public decimal TransferenciasEnviadas { get; set; } // Pagos realizados
        public decimal TransferenciasRecibidas { get; set; } // Pagos recibidos
        
        // FÓRMULA: Lo que pagué - Lo que consumí + Lo que transferí - Lo que cobré
        public decimal SaldoNeto => TotalPagadoBolsillo - TotalConsumidoDebe + TransferenciasEnviadas - TransferenciasRecibidas;
    }
}