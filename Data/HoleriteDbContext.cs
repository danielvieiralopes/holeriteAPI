


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


        public DbSet<ApplicationUser> Usuarios { get; set; }
        public DbSet<Holerite> Holerites { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

    }
}