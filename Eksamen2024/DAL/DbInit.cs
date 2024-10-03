using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Eksamen2024.Models;

namespace Eksamen2024.DAL
{
    public static class DBInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            ItemDbContext context = serviceScope.ServiceProvider.GetRequiredService<ItemDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User { Username = "Alice Hansen", Email = "alice@example.com" },
                    new User { Username = "Bob Johansen", Email = "bob@example.com" }
                };
                context.AddRange(users);
                context.SaveChanges();
            }

            if (!context.Pinpoints.Any())
            {
                var pinpoints = new List<Pinpoint>
                {
                    new Pinpoint { Name = "Central Park", Latitude = 40.785091, Longitude = -73.968285, UserId = 1 },
                    new Pinpoint { Name = "Statue of Liberty", Latitude = 40.689247, Longitude = -74.044502, UserId = 2 }
                };
                context.AddRange(pinpoints);
                context.SaveChanges();
            }

            if (!context.Comments.Any())
            {
                var comments = new List<Comment>
                {
                    new Comment { Text = "This is a great place!", UserId = 1, PinpointId = 1 },
                    new Comment { Text = "Had a wonderful time!", UserId = 2, PinpointId = 2 }
                };
                context.AddRange(comments);
                context.SaveChanges();
            }

            // You can add more seeding logic here for additional entities like Pinpoints, etc.
        }
    }
}
