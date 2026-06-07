namespace SupportModule.Contracts.TicketCategories.Responses;

public record TicketCategoryResponse(
    string CategoryId,
    string Name,
    string Description,
    bool IsActive,
    string DefaultPriority,
    IReadOnlyCollection<string> RequiredSkillIds);
