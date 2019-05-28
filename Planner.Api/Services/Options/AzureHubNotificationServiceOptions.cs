using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public class AzureHubNotificationServiceOptions
    {
        public string ConnectionString { get; set; }
        public string HubName { get; set; }
    }
}
