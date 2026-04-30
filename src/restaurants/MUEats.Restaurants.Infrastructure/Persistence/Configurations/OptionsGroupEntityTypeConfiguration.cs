using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Restaurants.Core.Domain.Menu.Entities;

namespace MUEats.Restaurants.Infrastructure.Persistence.Configurations;

public class OptionsGroupEntityTypeConfiguration : IEntityTypeConfiguration<OptionsGroup>
{
    public void Configure(EntityTypeBuilder<OptionsGroup> builder)
    {
        builder.ToTable("options_groups", "catalog");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(128);
    }
}