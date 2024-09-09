using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eksamen2024.Data;

namespace Eksamen2024.Repositories
{
    public class Repository<T> : GenRepository<T> where <T> : class
    {
        // Refering to the database context and represent the entity in the database
        protected readonly ItemDbContext _context; 
        private readonly DbSet<T> _dbSet;
        // Creating a constructor for the context
        public Repository(ItemDbContext context)
        {
            _context = context;
            // Accessing the dbset for Entities type T
            _dbSet = context.Set<T>();
        }
        // Retrieving all the entitites
        public async Task<IEnumerable<T>> GetAllSync
        {
            //Using return await so it doesnt interfere with the main thread.
            return await _dbSet.ToListAsync();
        }
        // Retrieving entity by its id
        public async Task<T> GetByIdSync(int Id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddBySync(T entity)
        {
            // Add the entity with await and change the database
            await _dbSet.AddBySync(entity);
            await _context.SaveChanges();
        }
        public async Task UpdateBySync(T entity)
        {
            // Mark entity for update and change the database with await
            _dbSet.UpdateBySync(entity);
            await _context.SaveChanges();
        }
        public async Task DeleteBySync(T entity)
        {
            //Mark entity for delete and change database with await
            _dbSet.DeleteBySync(entity);
            await _context.SaveChanges();
        }
    }
}