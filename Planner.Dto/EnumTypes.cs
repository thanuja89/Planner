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

    public enum SignUpErrorType
    {
        None,
        UsernameExists,
        EmailExists,
        ServerError,
        Other
    }
}
