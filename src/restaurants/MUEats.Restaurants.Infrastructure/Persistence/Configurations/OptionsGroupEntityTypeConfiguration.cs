using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Restaurants.Core.Domain.Menu.Entities;

namespace MUEats.Restaurants.Infrastructure.Persistence.Configurations;

public class OptionsGroupEntityTypeConfiguration : IEntityTypeConfiguration<OptionsGroup>
{
    public void Configure(EntityTypeBuilder<OptionsGroup> builder)
    {
        builder.ToTable("options_groups");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        
        builder.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(128);
        
        builder.HasMany(x => x.ItemOptions)
            .WithOne()
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}