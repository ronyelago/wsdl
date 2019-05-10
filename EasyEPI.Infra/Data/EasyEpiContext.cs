using EasyEPI.Infra.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyEPI.Infra.Data
{
    public class EasyEpiContext : DbContext
    {
        public EasyEpiContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<L_FUNCIONARIOS> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Persist Security Info=False;User ID=sa;Initial Catalog=EasyEPIDesenvolvimento;Data Source=200.219.235.40,5001");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
