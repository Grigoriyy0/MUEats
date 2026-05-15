using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Restaurants.Core.Projections.Order;

namespace MUEats.Restaurants.Infrastructure.Persistence.Configurations;

public class OrderSnapshotEntityTypeConfiguration : IEntityTypeConfiguration<OrderSnapshot>
{
    public void Configure(EntityTypeBuilder<OrderSnapshot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.OrderItems)
            .WithOne(y => y.Order)
            .HasForeignKey(z => z.OrderId);

        builder.Property(x => x.Status)
            .HasConversion<string>();
    }
}