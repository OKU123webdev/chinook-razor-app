using Microsoft.EntityFrameworkCore;

namespace ChinookContext{
    public class ChinookDatabase : DbContext{
        public DbSet<Album> Albums {get; set;}
        public DbSet<Artist> Artists {get; set;}
        public DbSet<Track> Tracks {get; set;}
        public DbSet<Genre> Genres {get; set;}


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename = chinook.db");
        }
    }
}