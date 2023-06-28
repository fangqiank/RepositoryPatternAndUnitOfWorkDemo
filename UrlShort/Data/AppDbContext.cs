using Microsoft.EntityFrameworkCore;
using UrlShort.Models;

namespace UrlShort.Data
{
    public class AppDbContext:  DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        
        public virtual DbSet<UrlManagerment> Urls { get; set; }
    }
}
