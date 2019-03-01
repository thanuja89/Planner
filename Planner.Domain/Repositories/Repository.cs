using Microsoft.EntityFrameworkCore;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Domain.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly PlannerDbContext Context;

        protected DbSet<T> Entities => Context.Set<T>();

        public Repository(PlannerDbContext context)
        {
            Context = context;
        }

        public Task<T> FindAsync(Guid id)
        {
            return Entities.FindAsync(id);
        }

        public Task<T> GetByIdAsync(Guid id)
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

        public Task AddRangeAsync(IEnumerable<T> entities)
        {
            return Entities.AddRangeAsync(entities);
        }

        public void Delete(T entity)
        {
            Entities.Remove(entity);
        }

        public async Task Delete(Guid id)
        {
            var entity = await Entities.FindAsync(id);
            Entities.Remove(entity);
        }
    }
}
