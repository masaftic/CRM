using SalesModule.Contracts.Deals.Requests;
using SalesModule.Contracts.Deals.Responses;
using SalesModule.Domain;
using Microsoft.EntityFrameworkCore;

namespace SalesModule.Infrastructure.Data.Queries;

public interface IDealsQueries
{
    Task<PagedResponse<DealResponse>> GetDeals(GetDealsRequest request);
    Task<DealResponse?> GetDeal(string dealId);
}


public class DealsQueries(SalesDbContext db) : IDealsQueries
{
    public async Task<PagedResponse<DealResponse>> GetDeals(GetDealsRequest request)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var query = db.Deals
            .AsNoTracking()
            .Include(x => x.SnapshotStages)
            .Include(x => x.DealMovements)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x => x.Name.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.PipelineId))
        {
            query = query.Where(x => x.PipelineId == PipelineId.Create(request.PipelineId));
        }

        if (!string.IsNullOrWhiteSpace(request.StageId))
        {
            query = query.Where(x => x.CurrentStageId == StageId.Create(request.StageId));
        }

        if (!string.IsNullOrWhiteSpace(request.Outcome) && Enum.TryParse<DealOutcome>(request.Outcome, true, out var outcome))
        {
            query = query.Where(x => x.Outcome == outcome);
        }

        if (request.ContactId is not null)
        {
            query = query.Where(x => x.ContactId == request.ContactId);
        }

        if (request.SalesPersonId is not null)
        {
            query = query.Where(x => x.SalesPersonId == request.SalesPersonId);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<DealResponse>
        {
            Items = items.Select(x => x.ToResponse()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<DealResponse?> GetDeal(string dealId)
    {
        var deal = await db.Deals
            .AsNoTracking()
            .Include(x => x.SnapshotStages)
            .Include(x => x.DealMovements)
            .FirstOrDefaultAsync(x => x.Id == DealId.Create(dealId));

        return deal?.ToResponse();
    }
}

public static class DealQueryExtensions
{
    extension(Deal deal)
    {
        public DealResponse ToResponse()
        {
            var currentStage = deal.SnapshotStages.First(x => x.StageId == deal.CurrentStageId);
            var forecastedValue = deal.ForecastedValue;

            return new DealResponse
            {
                Id = deal.Id,
                ContactId = deal.ContactId,
                SalesPersonId = deal.SalesPersonId,
                Name = deal.Name,
                ExpectedCloseDate = deal.ExpectedCloseDate,
                PipelineId = deal.PipelineId,
                CurrentStageId = deal.CurrentStageId,
                CurrentStageName = currentStage.Name,
                CurrentStageProbability = currentStage.Probability,
                Outcome = deal.Outcome.ToString(),
                Value_Amount = deal.Value.Amount,
                Value_Currency = deal.Value.Currency,
                ForecastedValue_Amount = forecastedValue.Amount,
                ForecastedValue_Currency = forecastedValue.Currency,
                Stages = deal.SnapshotStages
                    .OrderBy(x => x.Order)
                    .Select(x => new DealStageResponse
                    {
                        StageId = x.StageId,
                        Name = x.Name,
                        Order = x.Order,
                        Probability = x.Probability
                    })
                    .ToList(),
                Movements = deal.DealMovements
                    .OrderByDescending(x => x.Timestamp)
                    .Select(ToMovementResponse)
                    .ToList()
            };
        }
    }

    private static DealMovementResponse ToMovementResponse(DealMovement movement)
        => movement switch
        {
            StageChange stageChange => new DealMovementResponse
            {
                Id = stageChange.Id,
                Type = nameof(StageChange),
                Timestamp = stageChange.Timestamp,
                FromStageId = stageChange.FromStageId,
                ToStageId = stageChange.ToStageId
            },
            TerminalMovement terminalMovement => new DealMovementResponse
            {
                Id = terminalMovement.Id,
                Type = nameof(TerminalMovement),
                Timestamp = terminalMovement.Timestamp,
                Outcome = terminalMovement.Outcome.ToString()
            },
            DealReopen dealReopen => new DealMovementResponse
            {
                Id = dealReopen.Id,
                Type = nameof(DealReopen),
                Timestamp = dealReopen.Timestamp,
                ReturnedToStageId = dealReopen.ReturnedToStageId
            },
            _ => new DealMovementResponse
            {
                Id = movement.Id,
                Type = movement.GetType().Name,
                Timestamp = movement.Timestamp
            }
        };
}
