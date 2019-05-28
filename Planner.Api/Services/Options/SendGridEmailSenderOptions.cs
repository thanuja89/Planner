using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public class SendGridEmailSenderOptions
    {
        public string APIKey { get; set; }
        public string FromEmail { get; set; }
    }
}
