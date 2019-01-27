using Planner.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Domain.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        Task<T> FindAsync(long id);
        Task<T> GetByIdAsync(long id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task Delete(long id);
    }
}
