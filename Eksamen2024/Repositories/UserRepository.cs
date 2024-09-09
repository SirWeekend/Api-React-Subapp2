using Eksamen2024.Data;
using Eksamen2024.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Eksamen2024.Repositories
{
    public class UserRepository : Repository<Users>, GenRepository
    {
        public UserRepository(ItemDbContext context) : base(context){ }
        // Method to find the user by email
        public async Task<Users> GetByEmail(string Email)
        {
            // Using LINQ to locate a user based on their email adress
            return await _context.Users.FirstOrDefault(u => u.Email = Email)
        }
    }
}