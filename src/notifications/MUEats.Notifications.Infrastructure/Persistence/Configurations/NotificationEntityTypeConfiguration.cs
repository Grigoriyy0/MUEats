using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Notifications.Core.Domain;

namespace MUEats.Notifications.Infrastructure.Persistence.Configurations;

public class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(x => x.SentAt)
            .HasConversion<string>();
    }
}