namespace Application.Models.Requests
{
    /// <summary>
    /// Request para Admin: debe indicar explícitamente el participanteId de quien pagó.
    /// </summary>
    public class GastoPersonalizadoAdminRequest : GastoPersonalizadoRequest
    {
        public int ParticipanteId { get; set; }
    }
}
