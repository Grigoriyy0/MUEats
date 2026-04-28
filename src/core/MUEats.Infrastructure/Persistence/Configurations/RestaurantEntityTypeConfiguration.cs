using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Core.Domain.Restaurant;

namespace MUEats.Infrastructure.Persistence.Configurations;

public sealed class RestaurantEntityTypeConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasMany(x => x.FoodItems)
            .WithOne(x => x.Restaurant)
            .HasForeignKey(x => x.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);;
    }
}