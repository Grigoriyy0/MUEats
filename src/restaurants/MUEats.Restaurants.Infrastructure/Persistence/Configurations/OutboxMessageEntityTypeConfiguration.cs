using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Restaurants.Infrastructure.Persistence.Outbox;

namespace MUEats.Restaurants.Infrastructure.Persistence.Configurations;

public class OutboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.Property(p => p.Status)
            .HasConversion<string>();
    }
}