namespace Application.Models.Requests
{
    /// <summary>
    /// Request para User: actualiza un gasto con división igualitaria.
    /// El participante que pagó se mantiene o cambia, pero se resuelve desde el token JWT.
    /// </summary>
    public class ActualizarGastoIgualitarioRequest
    {
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Categoria { get; set; }
        public string? Comprobante { get; set; }
        public List<int> ParticipantesIds { get; set; } = [];
    }
}
