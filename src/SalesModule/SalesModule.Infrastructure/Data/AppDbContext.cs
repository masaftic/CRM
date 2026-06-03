using BuildingBlocks.Domain;
using SalesModule.Domain;
using Microsoft.EntityFrameworkCore;
using Thinktecture;
using Shared.Infrastructure.Data;
using Shared.Infrastructure.Data.Outbox;
using System.Data;
using SalesModule.Infrastructure.Data.ReadModels;

namespace SalesModule.Infrastructure.Data;

public class SalesDbContext(DbContextOptions options) : DbContext(options),
    IOutboxDbContext,
    IRepository<Deal, DealId>,
    IRepository<Pipeline, PipelineId>,
    ISalesUnitOfWork
{
    public const string DEFAULT_SCHEMA = "sales";

    public DbSet<Deal> Deals { get; set; }
    public DbSet<Pipeline> Pipelines { get; set; }
    public DbSet<StageSnapshot> StageSnapshots { get; set; }
    public DbSet<DealMovement> DealMovements { get; set; }
    public DbSet<Stage> Stages { get; set; }
    public DbSet<SalesContact> Contacts { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DEFAULT_SCHEMA);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);
        modelBuilder.AddThinktectureValueConverters();

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.ToTable("Outbox", DEFAULT_SCHEMA);
            b.HasKey(o => o.Id);

            b.Property(o => o.Type).HasConversion(
                t => t.AssemblyQualifiedName,
                s => Type.GetType(s!)!
            );

            b.Property(o => o.JsonData).IsRequired();
        });

        modelBuilder.Entity<SalesContact>(b =>
        {
            b.ToTable("Contacts", DEFAULT_SCHEMA);
            b.HasKey(c => c.ContactId);

            b.Property(c => c.Name).HasMaxLength(200).IsRequired();
            b.Property(c => c.CompanyName).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Ignore<IDomainEvent>();

        base.OnModelCreating(modelBuilder);
    }

    public IRepository<TAggregate, TKey> GetRepository<TAggregate, TKey>() where TAggregate : IAggregate => (IRepository<TAggregate, TKey>)this;

    #region Repositories

    public Task<Deal?> TryFindAsync(DealId id, CancellationToken cancellationToken = default)
        => Deals
            .Include(d => d.SnapshotStages)
            .Include(d => d.DealMovements)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    public void Add(Deal aggregate) => Deals.Add(aggregate);
    public void Delete(Deal aggregate) => Deals.Remove(aggregate);


    public Task<Pipeline?> TryFindAsync(PipelineId id, CancellationToken cancellationToken = default)
        => Pipelines
            .Include(p => p.Stages)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    public void Add(Pipeline aggregate) => Pipelines.Add(aggregate);
    public void Delete(Pipeline aggregate) => Pipelines.Remove(aggregate);

    #endregion
}
