using System.ComponentModel.DataAnnotations;

namespace Planner.Dto
{
    public class CreateAccountDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(4)] // For testing
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
