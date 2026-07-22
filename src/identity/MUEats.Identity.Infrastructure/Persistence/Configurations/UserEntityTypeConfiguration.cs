using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Identity.Core.Domain.User;

namespace MUEats.Identity.Infrastructure.Persistence.Configurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.FirstName)
            .HasMaxLength(128)
            .IsRequired();
        
        builder.Property(u => u.LastName)
            .HasMaxLength(128)
            .IsRequired();
        
        builder.Property(u => u.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.ComplexProperty(u => u.EmailAddress, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email_address") 
                .HasMaxLength(256)
                .IsRequired();
        });
        
        builder.OwnsMany(u => u.Attributes, attr =>
        {
            attr.ToTable("user_attributes");
            
            attr.WithOwner().HasForeignKey("user_id");

            attr.Property(a => a.Key)
                .HasMaxLength(100)
                .IsRequired();
            
            attr.Property(a => a.Value)
                .HasMaxLength(500)
                .IsRequired();

            attr.HasKey("user_id", "Key");
        });
    }
}