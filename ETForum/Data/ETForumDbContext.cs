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
        public DbSet<Pitanje> Pitanja { get; set; }
        public DbSet<Odgovor> Odgovori { get; set; }
        public DbSet<PitanjeLajk> PitanjeLajkovi { get; set; }
        public DbSet<OdgovorLajk> OdgovorLajkovi { get; set; }
        public DbSet<Komentar> Komentari { get; set; }
        public DbSet<Notifikacija> Notifikacije { get; set; }
        public DbSet<Prijateljstvo> Prijateljstva { get; set; }
        public DbSet<Dostignuce> Dostignuca { get; set; }
        public DbSet<OcjenaPredmeta> OcjenaPredmeta { get; set; }
        public DbSet<Poruka> Poruka { get; set; }
        public DbSet<Predmeti> Predmeti { get; set; }
        public DbSet<StudySession> StudySession { get; set; }
        public DbSet<LiveChat> LiveChat { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
            modelBuilder.Entity<Pitanje>().ToTable("Pitanje");
            modelBuilder.Entity<Odgovor>().ToTable("Odgovor");
            modelBuilder.Entity<PitanjeLajk>().ToTable("PitanjeLajk");
            modelBuilder.Entity<OdgovorLajk>().ToTable("OdgovorLajk");
            modelBuilder.Entity<Komentar>().ToTable("Komentar");
            modelBuilder.Entity<Notifikacija>().ToTable("Notifikacija");
            modelBuilder.Entity<Prijateljstvo>().ToTable("Prijateljstvo");
            modelBuilder.Entity<Dostignuce>().ToTable("Dostignuce");
            modelBuilder.Entity<OcjenaPredmeta>().ToTable("OcjenaPredmeta");
            modelBuilder.Entity<Poruka>().ToTable("Poruka");
            modelBuilder.Entity<Predmeti>().ToTable("Predmeti");
            modelBuilder.Entity<StudySession>().ToTable("StudySession");
            modelBuilder.Entity<LiveChat>().ToTable("LiveChat");

            //za unique lajkove
            modelBuilder.Entity<PitanjeLajk>()
                .HasIndex(pl => new { pl.korisnikId, pl.pitanjeId })
                .IsUnique();

            modelBuilder.Entity<OdgovorLajk>()
                .HasIndex(ol => new { ol.korisnikId, ol.odgovorId })
                .IsUnique();
        }
    }
}