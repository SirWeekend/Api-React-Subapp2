using Microsoft.EntityFrameworkCore;
using Eksamen2024.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eksamen2024.DAL
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ItemDbContext _db;

        public CommentRepository(ItemDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Comment>> GetAll()
        {
            return await _db.Comments.ToListAsync();
        }

        public async Task<Comment?> GetCommentById(int id)
        {
            return await _db.Comments.FindAsync(id);
        }

        public async Task Create(Comment comment)
        {
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();
        }

        public async Task Update(Comment comment)
        {
            _db.Comments.Update(comment);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var comment = await _db.Comments.FindAsync(id);
            if (comment == null)
            {
                return false;
            }

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
