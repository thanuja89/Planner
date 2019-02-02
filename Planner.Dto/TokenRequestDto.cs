using System.ComponentModel.DataAnnotations;

namespace Planner.Dto
{
    public class TokenRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
