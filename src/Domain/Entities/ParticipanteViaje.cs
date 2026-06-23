
using Domain.Enums;
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

        // Relaciones
        public Usuario? Usuario { get; set; }
        public Viaje? Viaje { get; set; }
        public ICollection<Pago> PagosRealizados { get; set; } = new List<Pago>(); // Pagos que yo envié
        public ICollection<Pago> PagosRecibidos { get; set; } = new List<Pago>();  // Pagos que yo cobré

        public ParticipanteViaje(int usuarioId, int viajeId, bool esOrganizador)
        {
            UsuarioId = usuarioId;
            ViajeId = viajeId;
            EsOrganizador = esOrganizador;
            SaldoTotal = 0;
            FechaIngreso = DateTime.Now;
            Estado = "Activo";
        }




    }
}