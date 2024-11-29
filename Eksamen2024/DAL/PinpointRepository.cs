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
            _logger = logger;
        }

        public async Task<IEnumerable<Pinpoint>> GetAll()
        {
            _logger.LogInformation("Fetching all pinpoints from database...");
            var pinpoints = await _db.Pinpoints
                .Include(p => p.User) // Inkluder brukerdata
                .ToListAsync();

            if (pinpoints == null || !pinpoints.Any())
            {
                _logger.LogWarning("No pinpoints found in the database.");
                return new List<Pinpoint>();
            }

            // Materialiser data for å unngå $ref-problemer
            var materializedPinpoints = pinpoints.Select(p => new Pinpoint
            {
                PinpointId = p.PinpointId,
                Name = p.Name,
                Description = p.Description,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                UserId = p.UserId
            }).ToList();

            _logger.LogInformation($"Fetched {materializedPinpoints.Count} pinpoints from the database.");
            return materializedPinpoints;
        }

        public async Task<Pinpoint?> GetPinpointById(int id)
        {
            _logger.LogInformation($"Fetching pinpoint with ID {id} from database...");
            var pinpoint = await _db.Pinpoints
                .Include(p => p.User) // Inkluder brukerdata
                .FirstOrDefaultAsync(p => p.PinpointId == id);

            if (pinpoint == null)
            {
                _logger.LogWarning($"Pinpoint with ID {id} not found.");
                return null;
            }

            // Materialiser pinpoint for å unngå $ref-problemer
            var materializedPinpoint = new Pinpoint
            {
                PinpointId = pinpoint.PinpointId,
                Name = pinpoint.Name,
                Description = pinpoint.Description,
                Latitude = pinpoint.Latitude,
                Longitude = pinpoint.Longitude,
                UserId = pinpoint.UserId
            };

            _logger.LogInformation($"Fetched pinpoint: {materializedPinpoint.Name}");
            return materializedPinpoint;
        }

        public async Task Create(Pinpoint pinpoint, int userId)
        {
            pinpoint.UserId = userId;
            _db.Pinpoints.Add(pinpoint);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Pinpoint created: {pinpoint.Name}");
        }

        public async Task Update(Pinpoint pinpoint)
        {
            try
            {
                _logger.LogInformation($"Updating pinpoint in database: ID={pinpoint.PinpointId}, Name={pinpoint.Name}");
                
                // Forsikre oss om at pinpoint er "tracked" av EF
                var existingEntity = await _db.Pinpoints.FirstOrDefaultAsync(p => p.PinpointId == pinpoint.PinpointId);
                if (existingEntity == null)
                {
                    _logger.LogError($"Pinpoint with ID {pinpoint.PinpointId} not found in database.");
                    throw new KeyNotFoundException($"Pinpoint with ID {pinpoint.PinpointId} does not exist.");
                }

                existingEntity.Name = pinpoint.Name;
                existingEntity.Description = pinpoint.Description;
                existingEntity.Latitude = pinpoint.Latitude;
                existingEntity.Longitude = pinpoint.Longitude;

                _db.Pinpoints.Update(existingEntity); // Oppdaterer objektet
                await _db.SaveChangesAsync();
                _logger.LogInformation("Pinpoint updated successfully in database.");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError($"Database update exception: {dbEx.InnerException?.Message ?? dbEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during update: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> Delete(int id)
        {
            var pinpoint = await _db.Pinpoints.FindAsync(id);
            if (pinpoint == null)
            {
                _logger.LogWarning($"Pinpoint with ID {id} not found for deletion.");
                return false;
            }

            _db.Pinpoints.Remove(pinpoint);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Pinpoint deleted: {id}");
            return true;
        }
    }
}
