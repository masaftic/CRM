using BuildingBlocks.Domain;
using SalesModule.Domain.Events;
using Thinktecture;

namespace SalesModule.Domain;


[ValueObject<string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
public sealed partial class DealId
{
    public const string Prefix = "deal";

    static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
    {
        if (!value.StartsWith(Prefix + "_"))
            validationError = new ValidationError($"DealId must start with '{Prefix}_'.");
        if (!Ulid.TryParse(value.AsSpan(Prefix.Length + 1), out _))
            validationError = new ValidationError("DealId must contain a valid ULID after the prefix.");
    }

    public static DealId NewId() => new($"{Prefix}_{Ulid.NewUlid()}");
}


public class Deal : AggregateRoot
{
    public DealId Id { get; init; }
    public Guid ContactId { get; init; }
    public Guid SalesPersonId { get; set; }

    public string Name { get; set; } = string.Empty;
    public Money Value { get; set; } = Money.Create(0, "USD");
    public DateTime ExpectedCloseDate { get; set; }

    public PipelineId PipelineId { get; init; }
    public List<StageSnapshot> SnapshotStages { get; init; } = [];

    public StageId CurrentStageId { get; private set; }
    public StageSnapshot CurrentStage => SnapshotStages.First(s => s.StageId == CurrentStageId);

    public Money ForecastedValue =>
        Outcome switch
        {
            DealOutcome.Won => Money.Create(Value.Amount, Value.Currency),
            DealOutcome.Lost => Money.Create(0, Value.Currency),
            DealOutcome.Open => Money.Create(Value.Amount * (CurrentStage.Probability / 100), Value.Currency),
            _ => throw new InvalidOperationException("Invalid deal outcome.")
        };


    private readonly List<DealMovement> _dealMovements = [];
    public IReadOnlyList<DealMovement> DealMovements => _dealMovements.AsReadOnly();

    public DealOutcome Outcome { get; private set; } = DealOutcome.Open;

    private Deal() { Id = default!; PipelineId = default!; CurrentStageId = default!; } // For ORM

    public Deal(DealId id, Guid contactId, Guid salesPersonId, string name, Money value, DateTime expectedCloseDate, Pipeline pipeline)
    {
        Id = id;
        ContactId = contactId;
        SalesPersonId = salesPersonId;
        Name = name;
        Value = value;
        ExpectedCloseDate = expectedCloseDate;
        SnapshotStages = pipeline.GetOrderedStages().Select(s => new StageSnapshot(id, s.Id, s.PipelineId, s.Name, s.Order, s.Probability)).ToList();
        PipelineId = pipeline.Id;

        if (SnapshotStages.Count == 0)
            throw new ArgumentException("Pipeline must have at least one stage.");

        CurrentStageId = SnapshotStages[0].StageId;

        RaiseDomainEvent(new DealCreated(Id, ContactId, Name, Value.Amount, PipelineId));
    }

    public void TransitionToStage(StageId newStageId)
    {
        if (Outcome != DealOutcome.Open)
            throw new InvalidOperationException("Cannot move a deal that is not open.");
        if (!SnapshotStages.Any(s => s.StageId == newStageId))
            throw new InvalidOperationException("New stage must be part of the deal's pipeline.");

        var oldStageId = CurrentStageId;
        CurrentStageId = newStageId;

        _dealMovements.Add(new StageChange
        {
            Id = Guid.NewGuid(),
            DealId = Id,
            FromStageId = oldStageId,
            ToStageId = newStageId
        });

        RaiseDomainEvent(new DealTransitioned(Id, oldStageId, newStageId));
    }

    public void MoveForward()
    {
        var currentIndex = SnapshotStages.FindIndex(s => s.StageId == CurrentStageId);
        if (currentIndex == SnapshotStages.Count - 1)
            throw new InvalidOperationException("Deal is already in the last stage.");

        TransitionToStage(SnapshotStages[currentIndex + 1].StageId);
    }

    public void MoveBackward()
    {
        var currentIndex = SnapshotStages.FindIndex(s => s.StageId == CurrentStageId);
        if (currentIndex == 0)
            throw new InvalidOperationException("Deal is already in the first stage.");

        TransitionToStage(SnapshotStages[currentIndex - 1].StageId);
    }

    public void MarkAsWon()
    {
        if (Outcome == DealOutcome.Lost)
            throw new InvalidOperationException("Cannot mark a lost deal as won.");

        Outcome = DealOutcome.Won;
        _dealMovements.Add(new TerminalMovement
        {
            Id = Guid.NewGuid(),
            DealId = Id,
            Outcome = Outcome
        });

        RaiseDomainEvent(new DealWon(Id, ContactId, Name, Value.Amount, Value.Currency));
    }

    public void MarkAsLost()
    {
        if (Outcome == DealOutcome.Won)
            throw new InvalidOperationException("Cannot mark a won deal as lost.");

        Outcome = DealOutcome.Lost;
        _dealMovements.Add(new TerminalMovement
        {
            Id = Guid.NewGuid(),
            DealId = Id,
            Outcome = Outcome
        });

        RaiseDomainEvent(new DealLost(Id));
    }

    public void Reopen(StageId returnedToStageId)
    {
        if (Outcome == DealOutcome.Open)
            throw new InvalidOperationException("Deal is already open.");
        if (!SnapshotStages.Any(s => s.StageId == returnedToStageId))
            throw new InvalidOperationException("Returned to stage must be part of the deal's pipeline.");

        Outcome = DealOutcome.Open;
        CurrentStageId = returnedToStageId;
        _dealMovements.Add(new DealReopen
        {
            Id = Guid.NewGuid(),
            DealId = Id,
            ReturnedToStageId = returnedToStageId
        });

        RaiseDomainEvent(new DealReopened(Id));
    }
}
