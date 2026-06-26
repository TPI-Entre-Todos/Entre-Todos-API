namespace Application.Models.Requests
{
    /// <summary>
    /// Request para User: el participante que pagó se resuelve automáticamente desde el token JWT.
    /// </summary>
    public class GastoPersonalizadoRequest
    {
        public int ViajeId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Categoria { get; set; }
        public string? Comprobante { get; set; }
        public List<ParticipanteMontoItem> Participantes { get; set; } = [];
    }

    public class ParticipanteMontoItem
    {
        public int ParticipanteId { get; set; }
        /// <summary>Monto exacto que le corresponde a este participante.</summary>
        public decimal Monto { get; set; }
    }
}
