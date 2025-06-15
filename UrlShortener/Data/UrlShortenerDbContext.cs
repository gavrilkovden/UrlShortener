namespace UrlShortener.Data
{
    using Microsoft.EntityFrameworkCore;
    using UrlShortener.Models;

    public class UrlShortenerDbContext : DbContext
    {
        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : base(options)
        {
        }

        public DbSet<UrlEntry> UrlEntries => Set<UrlEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrlEntry>()
                .HasIndex(u => u.ShortCode)
                .IsUnique();
        }
    }
}
