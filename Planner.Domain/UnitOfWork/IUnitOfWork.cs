using System.Threading.Tasks;

namespace Planner.Domain.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
