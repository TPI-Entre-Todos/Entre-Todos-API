namespace Application.Models.Requests
{
    /// <summary>
    /// Request para Admin: actualiza un gasto con división por porcentaje.
    /// El admin puede indicar explícitamente el participanteId de quien pagó.
    /// </summary>
    public class ActualizarGastoPorPorcentajeAdminRequest : ActualizarGastoPorPorcentajeRequest
    {
        public int ParticipanteId { get; set; }
    }
}
