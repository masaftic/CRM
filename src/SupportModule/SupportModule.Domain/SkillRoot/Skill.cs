using BuildingBlocks.Domain;
using Thinktecture;

namespace SupportModule.Domain.SkillRoot;


[ValueObject<Guid>]
public partial struct SkillId
{
    public static SkillId New() => Create(Guid.NewGuid());
}


public class Skill : AggregateRoot
{
    public SkillId Id { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private Skill() { }

    public Skill(string name, bool isActive)
    {
        Id = SkillId.New();
        Name = name;
        IsActive = isActive;
    }
}
