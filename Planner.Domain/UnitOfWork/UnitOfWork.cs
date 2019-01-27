using System.Threading.Tasks;

namespace Planner.Domain.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PlannerDbContext _context;

        public UnitOfWork(PlannerDbContext context)
        {
            _context = context;
        }

        public Task CompleteAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
