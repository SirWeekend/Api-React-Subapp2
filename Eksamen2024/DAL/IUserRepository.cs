using Eksamen2024.Models;

namespace Eksamen2024.DAL;


public interface IUserRepository
{
    Task<IEnumerable<Users>> GetAll();
    Task<Users?> GetUserById(int id);
    Task Create(Users users);
    Task Update (Users users);
    Task<bool> Delete(int id);
}
