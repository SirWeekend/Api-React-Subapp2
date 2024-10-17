using Microsoft.EntityFrameworkCore;
using Eksamen2024.Models;

namespace Eksamen2024.DAL;

public class ItemDbContext : DbContext
{
	public ItemDbContext(DbContextOptions<ItemDbContext> options) : base(options)
	{
        // Database.EnsureCreated();  // Remove this line if you use migrations
	}

	public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Pinpoint> Pinpoints { get; set; }
    public DbSet<Admin> Admins { get; set; }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
}