namespace SupportModule.Contracts.TicketCategories.Requests;

public record CreateTicketCategoryRequest(
    string Name,
    string Description,
    string DefaultPriority,
    List<Guid> RequiredSkillIds);

public record AddRequiredSkillRequest(Guid SkillId);
