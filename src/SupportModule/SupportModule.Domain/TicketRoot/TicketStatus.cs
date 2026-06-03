namespace SupportModule.Domain.TicketRoot;

public enum TicketStatus
{
    Open,
    InProgress,
    WaitingForCustomer,
    Resolved, // problem fixed
    Closed // no customer interaction further
}
