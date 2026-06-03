namespace SalesModule.Contracts.Deals.Requests;

public record GetDealsRequest
{
    public string? SearchTerm { get; init; }
    public string? PipelineId { get; init; }
    public string? StageId { get; init; }
    public string? Outcome { get; init; }
    public Guid? ContactId { get; init; }
    public Guid? SalesPersonId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
