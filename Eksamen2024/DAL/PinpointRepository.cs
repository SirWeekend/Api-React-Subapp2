using Microsoft.EntityFrameworkCore;
using Eksamen2024.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Eksamen2024.DAL
{
    public class PinpointRepository : IPinpointRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PinpointRepository> _logger;

        public PinpointRepository(ApplicationDbContext db)
        {
            _db = db;
            _logger = _logger;
        }

        public async Task<IEnumerable<Pinpoint>> GetAll()
        {
            return await _db.Pinpoints
            .Include(p => p.User) // Include User entity to connect pinpoint with users
            .ToListAsync();
        }

        public async Task<Pinpoint?> GetPinpointById(int id)
        {
            return await _db.Pinpoints
            .Include(p => p.User) // Include User enity to connect pinpoint with users
            .FirstOrDefaultAsync(p => p.PinpointId == id);
        }

        public async Task Create(Pinpoint pinpoint, int userId)
        {
            // The logged-in user creates the new pinpoint
            pinpoint.UserId = userId;
            // Save the pinpoint
            _db.Pinpoints.Add(pinpoint);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Pinpoint pinpoint)
        {
            _db.Pinpoints.Update(pinpoint);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var pinpoint = await _db.Pinpoints.FindAsync(id);
            if (pinpoint == null)
            {
                return false;
            }

            _db.Pinpoints.Remove(pinpoint);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
