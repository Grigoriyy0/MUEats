using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Core.Domain.Order.Entities;

namespace MUEats.Infrastructure.Persistence.Configurations;

public class OrderSagaEntityTypeConfiguration : IEntityTypeConfiguration<OrderSaga>
{
    public void Configure(EntityTypeBuilder<OrderSaga> builder)
    {
        builder.HasKey(o => o.CorrelationId);

        builder.Property(o => o.State)
            .HasConversion<string>();
    }
}