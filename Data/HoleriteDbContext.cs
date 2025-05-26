


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
            modelBuilder.Entity<Funcionario>()
                .HasIndex(f => f.Cpf)
                .IsUnique(); 

            modelBuilder.Entity<Funcionario>()
                .HasMany(f => f.Holerites)
                .WithOne(h => h.Funcionario)
                .HasForeignKey(h => h.FuncionarioId);
        }
        
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Holerite> Holerites { get; set; }

    }
}