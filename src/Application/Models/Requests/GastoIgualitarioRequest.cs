namespace Application.Models.Requests
{
    /// <summary>
    /// Request para crear un gasto con división igualitaria.
    /// El sistema calcula automáticamente monto / cantidad de participantes.
    /// </summary>
    public class GastoIgualitarioRequest
    {
        public int ViajeId { get; set; }
        public int ParticipanteId { get; set; }  // Quién pagó
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Categoria { get; set; }
        public string? Comprobante { get; set; }

        /// <summary>
        /// Lista de IDs de participantes entre quienes se divide el gasto.
        /// </summary>
        public List<int> ParticipantesIds { get; set; } = [];
    }
}
