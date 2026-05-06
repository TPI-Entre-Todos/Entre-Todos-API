using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Infraestructure.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Viaje> Viajes { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}