using Microsoft.EntityFrameworkCore;
using HaulageApp.Models;

namespace HaulageApp.Data
{
    public class HaulageDbContext : DbContext
    {
        public HaulageDbContext(DbContextOptions<HaulageDbContext> options)
            : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }
    }
}