namespace Application.Models
{
    public class ParticipanteViajeCreateRequest
    {
        public int UsuarioId { get; set; }
        public int ViajeId { get; set; }
        public bool EsOrganizador { get; set; }
    }
}