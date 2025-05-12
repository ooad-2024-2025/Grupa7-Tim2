using Microsoft.EntityFrameworkCore;
using ETForum.Models;

namespace ETForum.Data
{
    public class ETForumDbContext : DbContext
    {
        public ETForumDbContext(DbContextOptions<ETForumDbContext> options)
            : base(options)
        {
        }

        public DbSet<Korisnik> Korisnici { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
            base.OnModelCreating(modelBuilder);
        }
    }
}
