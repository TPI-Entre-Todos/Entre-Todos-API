namespace Domain.Entities
{
    public class ParticipanteViaje
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ViajeId { get; set; }
        public bool EsOrganizador { get; set; }
        public decimal SaldoTotal { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Estado { get; set; }
        public string EstadoInvitacion { get; set; }

        // Relaciones
        public Usuario Usuario { get; set; }
        public Viaje Viaje { get; set; }
    }
}