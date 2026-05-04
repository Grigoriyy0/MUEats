using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Restaurants.Core.Domain.Menu.Entities.ValueObjects;

namespace MUEats.Restaurants.Infrastructure.Persistence.Configurations;

public class ItemOptionEntityTypeConfiguration : IEntityTypeConfiguration<ItemOption>
{
    public void Configure(EntityTypeBuilder<ItemOption> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();
    }
}