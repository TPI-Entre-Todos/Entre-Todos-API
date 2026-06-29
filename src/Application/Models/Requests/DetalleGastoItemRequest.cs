namespace Application.Models.Requests
{
    public class DetalleGastoItemRequest
    {
        public int ParticipanteId { get; set; }

        /// <summary>
        /// Solo para TipoDivision.Personalizado: monto exacto que le corresponde a este participante.
        /// </summary>
        public decimal? MontoIndividual { get; set; }

        /// <summary>
        /// Solo para TipoDivision.PorPorcentaje: porcentaje del total (ej: 80 = 80%).
        /// </summary>
        public decimal? Porcentaje { get; set; }
    }
}
