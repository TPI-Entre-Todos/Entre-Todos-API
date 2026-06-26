namespace Application.Models.Requests
{
    /// <summary>
    /// Request para User: el participante que pagó se resuelve automáticamente desde el token JWT.
    /// </summary>
    public class GastoPorPorcentajeRequest
    {
        public int ViajeId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Categoria { get; set; }
        public string? Comprobante { get; set; }
        public List<ParticipantePorcentajeItem> Participantes { get; set; } = [];
    }

    public class ParticipantePorcentajeItem
    {
        public int ParticipanteId { get; set; }
        /// <summary>Porcentaje del total. Ej: 80 = 80%</summary>
        public decimal Porcentaje { get; set; }
    }
}
