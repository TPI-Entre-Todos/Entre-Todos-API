using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;

namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Invitacion> Invitaciones { get; set; }
        public DbSet<Viaje> Viajes { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<ParticipanteViaje> ParticipantesViaje { get; set; }

        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Gasto> Gastos { get; set; }

        public DbSet<Notificacion> Notificaciones { get; set; }

        public ApplicationContext(
            DbContextOptions<ApplicationContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().HasData(CreateUsuarioSeed());
        }
        private Usuario[] CreateUsuarioSeed()
        {
            return new[]
            {
                    new Usuario { Id = 1, Nombre="Admin", Email="admin@entretodos.com",Password="Admin123!", FechaRegistro= new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), Rol= Rol.Admin }
            };
        }
    }

}