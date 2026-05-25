using BuildingBlocks.Domain;
using SalesModule.Domain.Events;
using Thinktecture;

namespace SalesModule.Domain;


[ValueObject<string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
public sealed partial class PipelineId
{
    public const string Prefix = "pipl";

    static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
    {
        if (!value.StartsWith(Prefix + "_"))
            validationError = new ValidationError($"PipelineId must start with '{Prefix}_'.");
        if (!Ulid.TryParse(value.AsSpan(Prefix.Length + 1), out _))
            validationError = new ValidationError("PipelineId must contain a valid ULID after the prefix.");
    }

    public static PipelineId NewId() => new($"{Prefix}_{Ulid.NewUlid()}");
}

public class Pipeline : AggregateRoot
{
    public PipelineId Id { get; init; }
    public string Name { get; private set; }

    private readonly List<Stage> _stages = [];
    public IReadOnlyList<Stage> Stages => _stages;

    private Pipeline() { Id = default!; Name = string.Empty; } // For ORM

    public Pipeline(PipelineId id, string name, List<Stage> stages)
    {
        Id = id;
        Name = name;

        _stages.AddRange(stages);

        NormalizeAndValidate();

        RaiseDomainEvent(new PipelineCreated(id, name));
    }

    public IOrderedEnumerable<Stage> GetOrderedStages()
        => _stages.OrderBy(s => s.Order);

    // -------------------------
    // Commands
    // -------------------------

    public void AddStage(Stage stage)
    {
        _stages.Add(stage);

        NormalizeAndValidate();

        RaiseDomainEvent(new StageAdded(Id, stage.Id));
    }

    public void RemoveStage(StageId stageId)
    {
        var stage = _stages.FirstOrDefault(s => s.Id == stageId)
            ?? throw new InvalidOperationException("Stage not found");

        _stages.Remove(stage);

        NormalizeAndValidate();

        RaiseDomainEvent(new StageRemoved(Id, stageId));
    }

    public void MoveStage(StageId stageId, int newIndex)
    {
        if (newIndex < 0 || newIndex >= _stages.Count)
            throw new ArgumentOutOfRangeException(nameof(newIndex));

        var stage = _stages.FirstOrDefault(s => s.Id == stageId)
            ?? throw new InvalidOperationException("Stage not found");

        _stages.Remove(stage);
        _stages.Insert(newIndex, stage);

        NormalizeAndValidate();

        RaiseDomainEvent(new StageReordered(Id));
    }

    public void UpdateStage(StageId stageId, Action<Stage> update)
    {
        var stage = _stages.FirstOrDefault(s => s.Id == stageId)
            ?? throw new InvalidOperationException("Stage not found");

        update(stage);

        NormalizeAndValidate();

        RaiseDomainEvent(new StageUpdated(Id, stageId));
    }

    // -------------------------
    // Invariants
    // -------------------------

    private void NormalizeAndValidate()
    {
        for (int i = 0; i < _stages.Count; i++)
            _stages[i].Order = i;

        ValidateProbabilities();
    }

    private void ValidateProbabilities()
    {
        if (_stages.Count == 0)
            return;

        if (_stages[0].Probability != 0)
            throw new ArgumentException("First stage must have 0% probability.");

        for (int i = 1; i < _stages.Count; i++)
        {
            if (_stages[i].Probability <= _stages[i - 1].Probability)
                throw new ArgumentException("Stage probabilities must be strictly increasing.");
        }
    }
}

