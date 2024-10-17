using Eksamen2024.Models;

namespace Eksamen2024.DAL;


public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetUserById(int id);
    Task Create(User user);
    Task Update (User user);
    Task<bool> Delete(int id);
}
