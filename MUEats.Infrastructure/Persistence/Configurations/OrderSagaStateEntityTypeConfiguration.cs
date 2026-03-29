using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Core.Domain.Order.Entities;

namespace MUEats.Infrastructure.Persistence.Configurations;

public class OrderSagaStateEntityTypeConfiguration : IEntityTypeConfiguration<OrderSagaState>
{
    public void Configure(EntityTypeBuilder<OrderSagaState> builder)
    {
        builder.HasKey(o => o.CorrelationId);

        builder.Property(o => o.State)
            .HasConversion<string>();
    }
}