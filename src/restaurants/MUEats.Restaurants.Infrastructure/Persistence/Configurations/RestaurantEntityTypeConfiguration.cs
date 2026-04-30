using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Restaurants.Core.Domain.Restaurant;
using MUEats.Restaurants.Core.Domain.Restaurant.ValueObjects;

namespace MUEats.Restaurants.Infrastructure.Persistence.Configurations;

public class RestaurantEntityTypeConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("restaurants");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .HasMaxLength(128)
            .IsRequired();
        
        builder.Property(c => c.Description)
            .HasMaxLength(256);

        builder.Property(x => x.BusinessHours)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions) null),
                v => JsonSerializer.Deserialize<BusinessHours>(v,  (JsonSerializerOptions) null)
                )
            .HasColumnType("jsonb")
            .HasColumnName("business_hours");
    }
}