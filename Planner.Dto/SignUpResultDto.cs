namespace Planner.Dto
{
    public class SignUpResultDTO
    {
        public static SignUpResultDTO Success(string userId)
            => new SignUpResultDTO() { UserId = userId, Succeeded = true };

        public static SignUpResultDTO Failed(SignUpErrorType errorType)
            => new SignUpResultDTO() { ErrorType = errorType, Succeeded = false };

        public string UserId { get; set; }
        public bool Succeeded { get; set; }
        public SignUpErrorType ErrorType { get; set; }
    }
}
