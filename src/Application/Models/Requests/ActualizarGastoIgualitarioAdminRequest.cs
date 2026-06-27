namespace Application.Models.Requests
{
    /// <summary>
    /// Request para Admin: actualiza un gasto con división igualitaria.
    /// El admin puede indicar explícitamente el participanteId de quien pagó.
    /// </summary>
    public class ActualizarGastoIgualitarioAdminRequest : ActualizarGastoIgualitarioRequest
    {
        public int ParticipanteId { get; set; }
    }
}
