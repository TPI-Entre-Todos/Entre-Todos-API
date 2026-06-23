using System;
using System.Collections.Generic;

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

        // Relaciones base
        public Usuario? Usuario { get; set; }
        public Viaje? Viaje { get; set; }

        // Relaciones cruzadas de Gastos y Detalles
        public ICollection<Gasto> GastosPagados { get; set; } = new List<Gasto>();
        public ICollection<DetalleGasto> DetallesGastoDebido { get; set; } = new List<DetalleGasto>();

        // Relaciones cruzadas de Pagos Directos
        public ICollection<Pago> PagosRealizados { get; set; } = new List<Pago>(); // Soluciona error 1
        public ICollection<Pago> PagosRecibidos { get; set; } = new List<Pago>();  // Soluciona error 2

        public ParticipanteViaje(int usuarioId, int viajeId, bool esOrganizador)
        {
            UsuarioId = usuarioId;
            ViajeId = viajeId;
            EsOrganizador = esOrganizador;
            SaldoTotal = 0;
            FechaIngreso = DateTime.Now;
            Estado = "Activo";
        }

        public ParticipanteViaje() { }
    }
}