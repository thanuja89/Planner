using System.ComponentModel.DataAnnotations;

namespace Planner.Dto
{
    public class ResetPasswordRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
        public string Password { get; set; }
    }
}
