using Domain.Entities;

namespace Application.Models
{
    public class ParticipanteViajeDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public int ViajeId { get; set; }
        public bool EsOrganizador { get; set; }
        public decimal SaldoTotal { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Estado { get; set; }
        public string EstadoInvitacion { get; set; }

        public static ParticipanteViajeDto Create(ParticipanteViaje entity)
        {
            return new ParticipanteViajeDto
            {
                Id = entity.Id,
                UsuarioId = entity.UsuarioId,
                NombreUsuario = entity.Usuario?.Nombre ?? "Desconocido",
                ViajeId = entity.ViajeId,
                EsOrganizador = entity.EsOrganizador,
                SaldoTotal = entity.SaldoTotal,
                FechaIngreso = entity.FechaIngreso,
                Estado = entity.Estado,
                EstadoInvitacion = entity.EstadoInvitacion
            };
        }
    }
}