using BuildingBlocks.Domain;
using SalesModule.Domain;
using Microsoft.EntityFrameworkCore;

namespace SalesModule.Infrastructure.Data;

public class SalesDbContext(DbContextOptions options) : DbContext(options),
    IRepository<Deal, DealId>,
    IRepository<Pipeline, PipelineId>,
    IUnitOfWork
{
    public DbSet<Deal> Deals { get; set; }
    public DbSet<Pipeline> Pipelines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() where TAggregate : IAggregate => (IRepository<TAggregate, TKey>)this;


    public void Add(Deal aggregate) => Deals.Add(aggregate);
    public void Delete(Deal aggregate) => Deals.Remove(aggregate);
    public Task<Deal?> TryFindAsync(DealId id, CancellationToken cancellationToken = default) => Deals.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Pipeline?> TryFindAsync(PipelineId id, CancellationToken cancellationToken = default) => Pipelines.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    public void Add(Pipeline aggregate) => Pipelines.Add(aggregate);
    public void Delete(Pipeline aggregate) => Pipelines.Remove(aggregate);
}
