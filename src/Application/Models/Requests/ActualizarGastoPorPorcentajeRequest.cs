namespace Application.Models.Requests
{
    /// <summary>
    /// Request para User: actualiza un gasto con división por porcentaje.
    /// El participante que pagó se resuelve desde el token JWT.
    /// </summary>
    public class ActualizarGastoPorPorcentajeRequest
    {
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Categoria { get; set; }
        public string? Comprobante { get; set; }
        public List<ParticipantePorcentajeItem> Participantes { get; set; } = [];
    }
}
