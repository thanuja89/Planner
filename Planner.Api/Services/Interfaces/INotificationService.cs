using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public interface INotificationService
    {
        Task NotifyAsync(string userId, string title = "", string body = "");
    }
}
