namespace Application.Models.Requests
{
    /// <summary>
    /// Request para User: el participante que pagó se resuelve automáticamente desde el token JWT.
    /// </summary>
    public class GastoIgualitarioRequest
    {
        public int ParticipanteId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Categoria { get; set; }
        public string? Comprobante { get; set; }
        public List<int> ParticipantesIds { get; set; } = [];
    }
}
