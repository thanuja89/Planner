namespace Planner.Dto
{
    public class AccountCreationResultDto
    {
        public string UserId { get; set; }
        public bool Succeeded { get; set; }
        public AccountCreationErrorType ErrorType { get; set; }
        public static AccountCreationResultDto Success(string userId)
            => new AccountCreationResultDto() { UserId = userId, Succeeded = true };

        public static AccountCreationResultDto Failed(AccountCreationErrorType errorType)
            => new AccountCreationResultDto() { ErrorType = errorType, Succeeded = false };
    }
}
