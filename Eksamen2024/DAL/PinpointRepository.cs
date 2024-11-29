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

        public PinpointRepository(ApplicationDbContext db, ILogger<PinpointRepository> logger)
        {
            _db = db;
            _logger = logger; // Logger er nå riktig initialisert
        }

        // Enkelt GetAll-metode
        public async Task<IEnumerable<Pinpoint>> GetAll()
        {
            _logger.LogInformation("Fetching all pinpoints."); // Logging
            var pinpoints = await _db.Pinpoints
                .Include(p => p.User) // Include User entity to connect pinpoint with users
                .ToListAsync();
            _logger.LogInformation($"Fetched {pinpoints.Count} pinpoints from database."); // Logging
            return pinpoints;
        }

        public async Task<Pinpoint?> GetPinpointById(int id)
        {
            _logger.LogInformation($"Fetching pinpoint with ID: {id}"); // Logging
            return await _db.Pinpoints
                .Include(p => p.User) // Include User entity to connect pinpoint with users
                .FirstOrDefaultAsync(p => p.PinpointId == id);
        }

        public async Task Create(Pinpoint pinpoint, int userId)
        {
            _logger.LogInformation($"Creating pinpoint: {pinpoint.Name} by user ID: {userId}"); // Logging
            pinpoint.UserId = userId;
            _db.Pinpoints.Add(pinpoint);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Pinpoint pinpoint)
        {
            _logger.LogInformation($"Updating pinpoint with ID: {pinpoint.PinpointId}"); // Logging
            _db.Pinpoints.Update(pinpoint);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            _logger.LogInformation($"Deleting pinpoint with ID: {id}"); // Logging
            var pinpoint = await _db.Pinpoints.FindAsync(id);
            if (pinpoint == null)
            {
                _logger.LogWarning($"Pinpoint with ID: {id} not found."); // Logging
                return false;
            }

            _db.Pinpoints.Remove(pinpoint);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
