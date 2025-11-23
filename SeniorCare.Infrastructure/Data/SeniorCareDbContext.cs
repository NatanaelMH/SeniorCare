using Microsoft.EntityFrameworkCore;
using SeniorCare.Domain.Entities;

namespace SeniorCare.Infrastructure.Data
{
    public class SeniorCareDbContext : DbContext
    {
        public SeniorCareDbContext(DbContextOptions<SeniorCareDbContext> options) : base(options) { }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Cita> Citas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(120);
                e.Property(x => x.Telefono).HasMaxLength(20);
                e.Property(x => x.Correo).HasMaxLength(120);
                e.Property(x => x.Direccion).HasMaxLength(200);
                e.Property(x => x.TipoSangre).HasMaxLength(5);
                e.Property(x => x.Cedula).HasMaxLength(25);
            });

            modelBuilder.Entity<Cita>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Proposito).HasMaxLength(200);

                e.HasOne(x => x.Paciente)
                 .WithMany(p => p.Citas)
                 .HasForeignKey(x => x.PacienteId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
