namespace SalesModule.Contracts.Deals.Requests;

public record CreateDealRequest
{
    public Guid ContactId { get; set; }  
    public Guid SalesPersonId { get; set; }
    public string PipelineId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public MoneyDto Value { get; set; } = new MoneyDto();
    public DateTime ExpectedCloseDate { get; set; }
}



public record MoneyDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}
