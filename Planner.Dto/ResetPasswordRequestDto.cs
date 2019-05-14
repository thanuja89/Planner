using System.ComponentModel.DataAnnotations;

namespace Planner.Dto
{
    public class ResetPasswordRequestDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Code { get; set; }
        public string Password { get; set; }
    }
}
