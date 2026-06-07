using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data.Outbox;
using Shared.Infrastructure.Data.ValueConverters;
using SupportModule.Domain.SkillRoot;
using SupportModule.Domain.SupportAgentProfileRoot;
using SupportModule.Domain.TicketCategoryRoot;
using SupportModule.Domain.TicketCommentRoot;
using SupportModule.Domain.TicketRoot;
using Thinktecture;

namespace SupportModule.Infrastructure.Data;

public class SupportDbContext(DbContextOptions<SupportDbContext> options) : DbContext(options), IOutboxDbContext
{
    public const string DEFAULT_SCHEMA = "support";

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<TicketCategory> TicketCategories { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<SupportAgentProfile> SupportAgentProfiles { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DEFAULT_SCHEMA);
        modelBuilder.AddThinktectureValueConverters();

        modelBuilder.Entity<Ticket>(b =>
        {
            b.HasKey(x => x.TicketId);
            b.Property(x => x.Title).HasMaxLength(300).IsRequired();
            b.Property(x => x.Description).HasMaxLength(4000).IsRequired();
            b.HasMany(x => x.Assignments).WithOne().HasForeignKey(x => x.TicketId);
            b.HasMany(x => x.Transitions).WithOne().HasForeignKey(x => x.TicketId);
            b.HasMany(x => x.PriorityChanges).WithOne().HasForeignKey(x => x.TicketId);
        });

        modelBuilder.Entity<TicketAssignment>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Reason).HasMaxLength(1000);
        });

        modelBuilder.Entity<TicketTransition>(b =>
        {
            b.HasKey(x => x.Id);
        });

        modelBuilder.Entity<TicketPriorityChange>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Reason).HasMaxLength(1000);
        });

        modelBuilder.Entity<TicketComment>(b =>
        {
            b.Property(x => x.Id).HasConversion(
                x => x.ToString(),
                x => TicketCommentId.Create(Ulid.Parse(x))
            );

            b.HasKey(x => x.Id);
            b.Property(x => x.Content).HasMaxLength(4000).IsRequired();
            b.HasIndex(x => x.TicketId);
        });

        modelBuilder.Entity<TicketCategory>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            b.PrimitiveCollection<List<SkillId>>("_requiredSkills").HasThinktectureValueConverter().HasColumnName("RequiredSkills");
        });

        modelBuilder.Entity<Skill>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<SupportAgentProfile>(b =>
        {
            b.HasKey(x => x.ProfileId);
            b.HasIndex(x => x.AgentId).IsUnique();
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.PrimitiveCollection<List<SkillId>>("_skillIds").HasThinktectureValueConverter().HasColumnName("SkillIds");
            b.PrimitiveCollection<List<TicketId>>("_activeTicketIds").HasThinktectureValueConverter().HasColumnName("ActiveTicketIds");
        });

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.ToTable("Outbox", DEFAULT_SCHEMA);
            b.HasKey(o => o.Id);
            b.Property(o => o.Type).HasConversion(
                t => t.AssemblyQualifiedName,
                s => Type.GetType(s!)!);
            b.Property(o => o.JsonData).IsRequired();
        });

        modelBuilder.Ignore<IDomainEvent>();

        base.OnModelCreating(modelBuilder);
    }
}


