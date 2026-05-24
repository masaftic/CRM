using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesModule.Domain;

namespace SalesModule.Infrastructure.Data.Configurations;

public class PlatformConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.Property<int>("_id").ValueGeneratedOnAdd();
        builder.HasKey("_id");

        builder.Property(p => p.Id).IsRequired();
        builder.HasIndex(p => p.Id).IsUnique();
    }
}
