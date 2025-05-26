


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
                .HasIndex(f => f.Cpf)
                .IsUnique(); 

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(f => f.Holerites)
                .WithOne(h => h.Usuario)
                .HasForeignKey(h => h.UsuarioId);
        }
        
        public DbSet<ApplicationUser> Usuarios { get; set; }
        public DbSet<Holerite> Holerites { get; set; }

    }
}