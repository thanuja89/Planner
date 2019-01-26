using System.Linq;
using System.Threading.Tasks;

namespace Planner.Domain.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T> FindAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
    }
}
