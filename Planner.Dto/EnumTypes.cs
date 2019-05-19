namespace Planner.Dto
{
    public enum Importance
    {
        Low,
        Medium,
        High
    }

    public enum Frequency
    {
        Never,
        EveryDay,
        Weekdays,
        Weekends,
        Weekly,
        Monthly,
        Yearly
    }

    public enum AccountCreationErrorType
    {
        None,
        UsernameExists,
        EmailExists,
        ServerError,
        Other
    }

    public enum TokenCreationErrorType
    {
        None,
        InvalidUsernameOrPassword,
        EmailNotConfirmed,
        ServerError
    }
}
