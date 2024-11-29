using Eksamen2024.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eksamen2024.DAL
{
    public interface IPinpointRepository
    {
        Task<IEnumerable<Pinpoint>> GetAll();
        Task<Pinpoint?> GetPinpointById(int id);
        Task Create(Pinpoint pinpoint, int userId);
        Task Update(Pinpoint pinpoint);
        Task<bool> Delete(int id);
    }
}
