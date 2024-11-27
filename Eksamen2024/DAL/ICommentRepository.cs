using Eksamen2024.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eksamen2024.DAL
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAll();
        Task<Comment?> GetCommentById(int id);
        Task Create(Comment comment);
        Task Update(Comment comment);
        Task<bool> Delete(int id);
    }
}
