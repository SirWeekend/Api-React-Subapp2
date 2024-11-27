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
            ApplicationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
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
                    new Pinpoint 
                    { 
                    PinpointId = 2, 
                    Name = "Oslo Opera House", 
                    Latitude = 59.9074,
                    Longitude = 10.7530,
                    UserId = 1,
                    Description = "The biggest opera house in Norway. It is the home of many cultural events." 
                    },
                    new Pinpoint 
                    { 
                    PinpointId = 1,    
                    Name = "Akershus Fortress",
                    Latitude = 59.9094, 
                    Longitude = 10.7388, 
                    UserId = 2, 
                    Description = "The biggest opera house in Norway. It is the home of many cultural events.",
                    }

                };
                context.AddRange(pinpoints);
                context.SaveChanges();
            }

            if (!context.Comments.Any())
            {
                var comments = new List<Comment>
                {
                    new Comment { CommentId = 1, Text = "This is a great place!", UserId = 1, PinpointId = 1 },
                    new Comment { CommentId = 2, Text = "Had a wonderful time!", UserId = 2, PinpointId = 2 }
                };
                context.AddRange(comments);
                context.SaveChanges();
            }
            

            // You can add more seeding logic here for additional entities like Pinpoints, etc.
        }
    }
}
