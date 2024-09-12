using Microsoft.EntityFrameworkCore;
using Eksamen2024.Models;

namespace Eksamen2024.DAL;

public class UserRepository : IUserRepository
{
    private readonly ItemDbContext _db;

    public UserRepository(ItemDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Users>> GetAll()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<Users?> GetUserById(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task Create(Users user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task Update(Users user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
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