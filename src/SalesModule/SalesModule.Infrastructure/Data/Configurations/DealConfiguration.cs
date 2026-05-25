using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesModule.Domain;

namespace SalesModule.Infrastructure.Data.Configurations;

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.HasKey(d => d.Id);

        builder.OwnsMany(d => d.SnapshotStages, sb =>
        {
            sb.Property<int>("Id").ValueGeneratedOnAdd();
            sb.HasKey("Id");
            sb.WithOwner().HasForeignKey(s => s.DealId);
        });

        builder.ComplexProperty(d => d.Value, mb =>
        {
            mb.Property(m => m.Amount).HasColumnName("Value_Amount");
            mb.Property(m => m.Currency).HasColumnName("Value_Currency");
        });

        builder.HasMany(d => d.DealMovements)
            .WithOne()
            .HasForeignKey(dm => dm.DealId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(d => d.CurrentStage);
    }
}



public class DealMovementConfiguration : IEntityTypeConfiguration<DealMovement>
{
    public void Configure(EntityTypeBuilder<DealMovement> builder)
    {
        builder.HasKey(dm => dm.Id);

        builder.UseTphMappingStrategy()
            .HasDiscriminator<string>("MovementType")
            .HasValue<StageChange>("StageChange")
            .HasValue<TerminalMovement>("TerminalMovement")
            .HasValue<DealReopen>("DealReopen");
    }
}

