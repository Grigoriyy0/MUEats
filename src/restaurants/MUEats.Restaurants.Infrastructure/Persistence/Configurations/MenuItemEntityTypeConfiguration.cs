using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Restaurants.Core.Domain.Menu.Entities;

namespace MUEats.Restaurants.Infrastructure.Persistence.Configurations;

public class MenuItemEntityTypeConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items", "catalog");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.HasMany(x => x.OptionsGroups)
            .WithOne()
            .HasForeignKey(x => x.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Metadata
            .FindNavigation(nameof(MenuItem.OptionsGroups))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}