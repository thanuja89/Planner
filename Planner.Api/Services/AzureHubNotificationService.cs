using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public class AzureHubNotificationService : INotificationService
    {
        private readonly AzureHubNotificationServiceOptions _options;
        private readonly ILogger<AzureHubNotificationService> _logger;

        public AzureHubNotificationService(IOptions<AzureHubNotificationServiceOptions> optionsAccessor
        , ILogger<AzureHubNotificationService> logger)
        {
            _options = optionsAccessor.Value;
            _logger = logger;
        }


        public async Task NotifyAsync(string userId, string deviceId)
        {
            try
            {
                NotificationHubClient hub = NotificationHubClient
                                        .CreateClientFromConnectionString(_options.ConnectionString, _options.HubName);

                string json = JsonConvert.SerializeObject(new
                {
                    data = new
                    {
                        fromDeviceId = deviceId
                    }
                });

                await hub.SendFcmNativeNotificationAsync(json, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in AzureHubNotificationService: {ex}");               
            }
        }
    }
}
