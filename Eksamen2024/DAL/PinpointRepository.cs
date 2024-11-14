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
        }

        public async Task<IEnumerable<Pinpoint>> GetAll()
        {
            return await _db.Pinpoints.ToListAsync();
        }

        public async Task<Pinpoint?> GetPinpointById(int id)
        {
            return await _db.Pinpoints.FindAsync(id);
        }

        public async Task Create(Pinpoint pinpoint)
        {
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
