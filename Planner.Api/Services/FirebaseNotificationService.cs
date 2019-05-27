using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public class FirebaseNotificationService : INotificationService
    {
        private readonly FirebaseNotificationServiceOptions _options;
        private readonly IRepository<Device> _deviceRepo;
        private readonly ILogger<FirebaseNotificationService> _logger;

        public FirebaseNotificationService(IOptions<FirebaseNotificationServiceOptions> optionsAccessor
            , IRepository<Device> deviceRepo
            , ILogger<FirebaseNotificationService> logger)
        {
            _options = optionsAccessor.Value;
            _deviceRepo = deviceRepo;
            _logger = logger;
        }

        public async Task NotifyAsync(string userId, string title = "", string body = "")
        {
            try
            {
                var ids = await GetDeviceIdsAsync(userId);

                var data = new
                {
                    registration_ids = ids, // Recipient device token
                    notification = new { title, body }
                };

                var jsonBody = JsonConvert.SerializeObject(data);

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
                {
                    httpRequest.Headers.TryAddWithoutValidation("Authorization", _options.ServerKey);
                    httpRequest.Headers.TryAddWithoutValidation("Sender", _options.SenderId);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    using (var httpClient = new HttpClient())
                    {
                        await httpClient.SendAsync(httpRequest);                       
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in Notify Service: {ex}");
            }
        }

        public Task<string[]> GetDeviceIdsAsync(string userId)
        {
            return _deviceRepo.GetAll()
                .Where(d => d.ApplicationUserId == userId)
                .Select(d => d.RegistrationId)
                .ToArrayAsync();
        } 
    }
}
