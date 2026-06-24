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
        public DbSet<DetalleGasto> DetallesGasto { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().HasData(CreateUsuarioSeed());
            // 1. Configuración de Relación: Un Gasto pertenece a un ParticipanteViaje (El que pagó)
            modelBuilder.Entity<Gasto>()
                .HasOne(g => g.Participante)
                .WithMany(p => p.GastosPagados)
                .HasForeignKey(g => g.ParticipanteId)
                .OnDelete(DeleteBehavior.Restrict); // Evita borrados en cascada infinitos

            // 2. Configuración de Relación: Un DetalleGasto pertenece al ParticipanteViaje que debe la plata
            modelBuilder.Entity<DetalleGasto>()
                .HasOne(dg => dg.Participante)
                .WithMany(p => p.DetallesGastoDebido)
                .HasForeignKey(dg => dg.ParticipanteId)
                .OnDelete(DeleteBehavior.Restrict); // Evita conflictos en base de datos

            // 3. Configuración de Relación opcional por prolijidad: Un DetalleGasto pertenece a un Gasto Maestro
            modelBuilder.Entity<DetalleGasto>()
                .HasOne(dg => dg.Gasto)
                .WithMany(g => g.DetallesGasto)
                .HasForeignKey(dg => dg.GastoId)
                .OnDelete(DeleteBehavior.Cascade); // Si borrás el Gasto, se borran automáticamente sus divisiones

            // Un Pago tiene un Remitente (El participante que transfiere el dinero)
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Remitente)
                .WithMany(pv => pv.PagosRealizados)
                .HasForeignKey(p => p.RemitenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un Pago tiene un Destinatario (El participante que recibe la transferencia)
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Destinatario)
                .WithMany(pv => pv.PagosRecibidos)
                .HasForeignKey(p => p.DestinatarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un Pago pertenece a un Viaje en específico
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Viaje)
                .WithMany(v => v.Pagos)
                .HasForeignKey(p => p.ViajeId)
                .OnDelete(DeleteBehavior.Cascade);

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