using BuildingBlocks.Domain;
using SupportModule.Domain.SkillRoot;
using SupportModule.Domain.TicketRoot;
using Thinktecture;

namespace SupportModule.Domain.TicketCategoryRoot;


[ValueObject<Guid>]
public partial struct TicketCategoryId
{
    public static TicketCategoryId New() => Create(Guid.NewGuid());
}


public class TicketCategory : AggregateRoot
{
    public TicketCategoryId Id { get; init; } = default;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public TicketPriority DefaultPriority { get; private set; }
    private readonly List<SkillId> _requiredSkills = [];
    public IReadOnlyCollection<SkillId> RequiredSkills => _requiredSkills.AsReadOnly();

    private TicketCategory() { }

    public TicketCategory(string name, string description, bool isActive, TicketPriority defaultPriority, List<SkillId> requiredSkills)
    {
        Id = TicketCategoryId.New();
        Name = name;
        Description = description;
        IsActive = isActive;
        DefaultPriority = defaultPriority;
        _requiredSkills.AddRange(requiredSkills);
    }

    public void RequireSkill(SkillId skillId)
    {
        if (!_requiredSkills.Contains(skillId))
            _requiredSkills.Add(skillId);
    }

    public void RemoveRequiredSkill(SkillId skillId)
    {
        var item = _requiredSkills.Find(x => x == skillId);
        if (item != default) _requiredSkills.Remove(item);
    }
}
