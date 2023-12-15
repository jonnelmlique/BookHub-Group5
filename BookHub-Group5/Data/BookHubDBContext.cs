using BookHub_Group5.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookHub_Group5.Data
{
    public class BookHubDBContext : DbContext
    {
        public BookHubDBContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<books> books { get; set; }
        public DbSet<sales> sales { get; set; }

    }
}
