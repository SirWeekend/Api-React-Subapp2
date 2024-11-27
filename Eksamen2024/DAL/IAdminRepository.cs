using Eksamen2024.Models;

namespace Eksamen2024.DAL;


public interface IAdminRepository
{
    Task<IEnumerable<Admin>> GetAllAdmins();
    Task<Admin?> GetAdminById(int id);
    Task CreateAdmin(Admin admin);
    Task UpdateAdmin(Admin admin);
    Task<bool> DeleteAdmin(int id);

    // User management methods
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> GetUserById(int id);
    Task CreateUser(User user);
    Task UpdateUser(User user);
    Task<bool> DeleteUser(int id);
}
