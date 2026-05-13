using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Restaurants.Infrastructure.Persistence.Inbox;

namespace MUEats.Restaurants.Infrastructure.Persistence.Configurations;

public class InboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.Property(p => p.Status)
            .HasConversion<string>();
    }
}