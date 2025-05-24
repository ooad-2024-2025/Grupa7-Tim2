using Microsoft.EntityFrameworkCore;
using ETForum.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ETForum.Data
{
    public class ETForumDbContext : IdentityDbContext<Korisnik>
    {
        public ETForumDbContext(DbContextOptions<ETForumDbContext> options)
            : base(options)
        {
        }

        public DbSet<Korisnik> Korisnici { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
        }
    }
}
