using BlazorCRUDWebApi.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorCRUDWebApi.Server.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<Driver> Drivers { get; set; }

    }
}
