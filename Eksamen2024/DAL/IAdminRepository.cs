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
    Task<IEnumerable<Users>> GetAllUsers();
    Task<Users?> GetUserById(int id);
    Task CreateUser(Users user);
    Task UpdateUser(Users user);
    Task<bool> DeleteUser(int id);
}
