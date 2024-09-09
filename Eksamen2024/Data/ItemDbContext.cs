using Microsoft.EntityFrameworkCore;
using Eksamen2024.Models;

namespace Eksamen2024.Data
{
    public class ItemDbContext : DbContext
    {
        public ItemDbContext(DbContextOptions<ItemDbContext> options) : base (options) { }
        public DbSet<Users> Users { get; set; }
        public DbSet<Pinpoint> Pinpoint { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}