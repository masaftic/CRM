namespace SalesModule.Contracts.Deals.Responses;

public class DealResponse
{
    public string Id { get; set; } = string.Empty;
    public Guid ContactId { get; set; }
    public Guid SalesPersonId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ExpectedCloseDate { get; set; }
    public string CurrentStageId { get; set; } = string.Empty;
    public string CurrentStageName { get; set; } = string.Empty;
    public decimal CurrentStageProbability { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public decimal Value_Amount { get; set; }
    public string Value_Currency { get; set; } = string.Empty;
    public decimal ForecastedValue_Amount { get; set; }
    public string ForecastedValue_Currency { get; set; } = string.Empty;
    public string PipelineId { get; set; } = string.Empty;
    public IReadOnlyList<DealStageResponse> Stages { get; set; } = [];
    public IReadOnlyList<DealMovementResponse> Movements { get; set; } = [];
}

public class DealStageResponse
{
    public string StageId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public decimal Probability { get; set; }
}

public class DealMovementResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
    public string? FromStageId { get; set; }
    public string? ToStageId { get; set; }
    public string? ReturnedToStageId { get; set; }
    public string? Outcome { get; set; }
}
