namespace SalesModule.Domain;

public abstract class DealMovement
{
    public Guid Id { get; init; }
    public required DealId DealId { get; set; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

public class StageChange : DealMovement
{
    public required StageId FromStageId { get; set; }
    public required StageId ToStageId { get; set; }
}

public class TerminalMovement : DealMovement
{
    public DealOutcome Outcome { get; set; }
}

public class DealReopen : DealMovement
{
    public required StageId ReturnedToStageId { get; set; }
}
