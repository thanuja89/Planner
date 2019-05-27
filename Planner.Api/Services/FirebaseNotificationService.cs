using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Planner.Api.Services.Interfaces;
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
        private readonly ILogger<FirebaseNotificationService> _logger;

        public FirebaseNotificationService(IOptions<FirebaseNotificationServiceOptions> optionsAccessor
            , ILogger<FirebaseNotificationService> logger)
        {
            _options = optionsAccessor.Value;
            _logger = logger;
        }

        public async Task<bool> NotifyAsync(string to, string title, string body)
        {
            try
            {
                var data = new
                {
                    to, // Recipient device token
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
                        var result = await httpClient.SendAsync(httpRequest);

                        if (result.IsSuccessStatusCode)
                        {
                            return true;
                        }
                        else
                        {
                            _logger.LogError($"Error sending notification. Status Code: {result.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in Notify Service: {ex}");
            }

            return false;
        }
    }
}
