using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eksamen2024.Repositories
{
    // Generic repository to make sure we can perform CRUD operations on any entity
    public interface GenRepository<T> where T : class
    {
        // Retriving all the entities
        Task<IEnumerable<T>> GetAllSync();
        // Retriving an entity by its Id
        Task<T> GetByIdSync(int Id);
        // Adding an entity
        Task AddBySync(T entity);
        // Updating an already existing entity
        Task UpdateBySync (T entity);
        // Deleting an entity
        Task DeleteBySync (T entity);

    }
}