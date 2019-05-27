using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Planner.Domain.Entities
{
    public class Device : BaseEntity
    {
        [Required]
        public string RegistrationId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
    }
}
