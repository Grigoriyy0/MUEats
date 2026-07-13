using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Discounts.Core.Discount;

namespace MUEats.Discounts.Infrastructure.Persistence.Configurations;

public class DiscountEntityTypeConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.TargetedRoles)
            .WithOne(y => y.Discount)
            .HasForeignKey(z => z.DiscountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Type)
            .HasConversion<string>();
    }
}