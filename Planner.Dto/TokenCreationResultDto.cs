namespace Planner.Dto
{
    public class TokenCreationResultDto
    {
        public TokenDto Token { get; set; }
        public bool Succeeded { get; set; }
        public string ValidationData { get; set; }
        public TokenCreationErrorType ErrorType { get; set; }

        public static TokenCreationResultDto Success(TokenDto token)
            => new TokenCreationResultDto() { Token = token, Succeeded = true };

        public static TokenCreationResultDto Failed(TokenCreationErrorType errorType, string userId = null)
            => new TokenCreationResultDto() { ErrorType = errorType, Succeeded = false, ValidationData = userId };
    }
}
