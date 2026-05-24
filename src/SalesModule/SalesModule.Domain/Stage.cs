using Thinktecture;

namespace SalesModule.Domain;



[ValueObject<string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
public sealed partial class StageId
{
    public const string Prefix = "stg";

    static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
    {
        if (!value.StartsWith(Prefix + "_"))
            validationError = new ValidationError($"StageId must start with '{Prefix}_'.");
        if (!Ulid.TryParse(value.AsSpan(Prefix.Length + 1), out _))
            validationError = new ValidationError("StageId must contain a valid ULID after the prefix.");
    }

    public static StageId NewId() => new($"{Prefix}_{Ulid.NewUlid()}");
}


public class Stage
{
    public StageId Id { get; init; }
    public PipelineId PipelineId { get; init; }

    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }

    public decimal Probability { get; set; }

    private Stage() { Id = default!; PipelineId = default!; } 

    public Stage(StageId id, PipelineId pipelineId, string name, int order, decimal probability)
    {
        Id = id;
        PipelineId = pipelineId;
        Name = name;
        Order = order;
        Probability = probability;
    }
}
