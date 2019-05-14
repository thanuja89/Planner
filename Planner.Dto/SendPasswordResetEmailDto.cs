using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Planner.Dto
{
    public class SendPasswordResetEmailDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
