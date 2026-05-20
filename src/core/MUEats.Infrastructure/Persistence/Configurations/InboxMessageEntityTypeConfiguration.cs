using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Core;

namespace MUEats.Infrastructure.Persistence.Configurations;

public class InboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.Property(x => x.Status)
            .HasConversion<string>();
    }
}