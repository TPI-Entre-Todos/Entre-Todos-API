using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Invitacion> Invitaciones { get; set; }
        public DbSet<Viaje> Viajes { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<ParticipanteViaje> ParticipantesViaje { get; set; }

        public DbSet<Pago> Pagos { get; set; }

        public ApplicationContext(
            DbContextOptions<ApplicationContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}