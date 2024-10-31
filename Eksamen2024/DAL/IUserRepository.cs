using Eksamen2024.Models;

namespace Eksamen2024.DAL;


public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetUserById(int id);
<<<<<<< HEAD
    Task Create(User users);
    Task Update (User users);
=======
    Task Create(User user);
    Task Update (User user);
>>>>>>> main
    Task<bool> Delete(int id);
}
