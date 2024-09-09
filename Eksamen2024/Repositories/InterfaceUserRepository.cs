using Eksamen2024.Models;
using System.Threading.Tasks;

namespace Eksamen2024.Repositories
{
    //Interface for the user repository and allowing methods for users
    public interface InterfaceUserRepository : GenRepository<Users>
    {
        // Possibility to get the user by their Email aswell
        Task<Users> GetByEmail (string Email);
    }
}