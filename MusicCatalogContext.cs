using Microsoft.EntityFrameworkCore;

namespace lab4 
{
    public class MusicCatalogContext : DbContext
    {
        public DbSet<MusicComposition> Catalog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=music_catalog.db");
        }
    }
}