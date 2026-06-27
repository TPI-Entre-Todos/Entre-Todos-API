namespace Application.Models.Requests
{
    /// <summary>
    /// Request para Admin: actualiza un gasto con división personalizada.
    /// El admin puede indicar explícitamente el participanteId de quien pagó.
    /// </summary>
    public class ActualizarGastoPersonalizadoAdminRequest : ActualizarGastoPersonalizadoRequest
    {
        public int ParticipanteId { get; set; }
    }
}
