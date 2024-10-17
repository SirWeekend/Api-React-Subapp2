using Microsoft.EntityFrameworkCore;
using Eksamen2024.Models;

namespace Eksamen2024.DAL;

public class AdminRepository : IAdminRepository
{
    private readonly ItemDbContext _db;

    public AdminRepository(ItemDbContext db)
    {
        _db = db;
    }

    // Methods to manage Admins
    public async Task<IEnumerable<Admin>> GetAllAdmins()
    {
        return await _db.Admins.ToListAsync();
    }

    public async Task<Admin?> GetAdminById(int id)
    {
        return await _db.Admins.FindAsync(id);
    }

    public async Task CreateAdmin(Admin admin)
    {
        _db.Admins.Add(admin);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAdmin(Admin admin)
    {
        _db.Admins.Update(admin);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAdmin(int id)
    {
        var admin = await _db.Admins.FindAsync(id);
        if (admin == null)
        {
            return false;
        }

        _db.Admins.Remove(admin);
        await _db.SaveChangesAsync();
        return true;
    }

    // Methods to manage Users (already implemented)
    public async Task<IEnumerable<Users>> GetAllUsers()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<Users?> GetUserById(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task CreateUser(Users user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateUser(Users user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }
}

