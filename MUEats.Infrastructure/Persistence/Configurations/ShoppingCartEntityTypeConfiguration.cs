using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Core.Domain.ShoppingCart;

namespace MUEats.Infrastructure.Persistence.Configurations;

public class ShoppingCartEntityTypeConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.CartItems)
            .WithOne(x => x.Cart)
            .HasForeignKey(x => x.FoodItemId)
            .OnDelete(DeleteBehavior.Cascade);;
    }
}