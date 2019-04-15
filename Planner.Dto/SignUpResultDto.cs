namespace Planner.Dto
{
    public class SignUpResultDTO
    {
        public string UserId { get; set; }
        public bool Succeeded { get; set; }
        public SignUpErrorType ErrorType { get; set; }
    }
}
