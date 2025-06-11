


using HoleriteApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HoleriteApi.Data
{
    public class HoleriteDbContext : IdentityDbContext<ApplicationUser>
    {
        public HoleriteDbContext(DbContextOptions<HoleriteDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Cpf)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.NomeFuncionario)
                .HasMaxLength(255);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Cpf)
                .HasMaxLength(450);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Holerites)
                .WithOne(h => h.Usuario)
                .HasForeignKey(h => h.UsuarioId);

            modelBuilder.Entity<Funcionario>()
                .Property(f => f.Nome)
                .HasMaxLength(255);

            modelBuilder.Entity<Funcionario>()
                .Property(f => f.Cpf)
                .HasMaxLength(450);

            modelBuilder.Entity<Holerite>()
                .Property(h => h.NomeFuncionarioExtraido)
                .HasMaxLength(255);
        }

        public override int SaveChanges()
        {
            SetUtcDateTimes();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetUtcDateTimes();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetUtcDateTimes()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                foreach (var property in entry.Properties)
                {
                    if (property.Metadata.ClrType == typeof(DateTime) && property.CurrentValue is DateTime dt)
                    {
                        property.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    }
                }
            }
        }


        public DbSet<ApplicationUser> Usuarios { get; set; }
        public DbSet<Holerite> Holerites { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

    }
}