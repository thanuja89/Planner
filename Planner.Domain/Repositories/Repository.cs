using Microsoft.EntityFrameworkCore;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Domain.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly PlannerDbContext _context;

        protected DbSet<T> Entities => _context.Set<T>();

        public Repository(PlannerDbContext context)
        {
            _context = context;
        }

        public Task<T> FindAsync(int id)
        {
            return Entities.FindAsync(id);
        }

        public Task<T> GetByIdAsync(int id)
        {
            return Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public IQueryable<T> GetAll()
        {
            return Entities;
        }

        public Task AddAsync(T entity)
        {
            return Entities.AddAsync(entity);
        }

        public void Update(T entity)
        {
            Entities.Update(entity);
        }

        public void Delete(T entity)
        {
            Entities.Remove(entity);
        }

        public async Task Delete(int id)
        {
            var entity = await Entities.FindAsync(id);
            Entities.Remove(entity);
        }
    }
}
