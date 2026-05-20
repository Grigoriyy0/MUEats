using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Restaurants.Core.Domain.Menu;

namespace MUEats.Restaurants.Infrastructure.Persistence.Configurations;

public class MenuEntityTypeConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("menus");

        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.MenuItems)
            .WithOne()
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Categories)
            .WithOne()
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Metadata
            .FindNavigation(nameof(Menu.MenuItems))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Metadata
            .FindNavigation(nameof(Menu.Categories))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}