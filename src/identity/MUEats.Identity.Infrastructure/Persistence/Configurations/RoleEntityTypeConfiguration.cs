using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Identity.Core.Domain.Role;

namespace MUEats.Identity.Infrastructure.Persistence.Configurations;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        
        builder.Property(x => x.RoleName)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.OwnsMany(x => x.Requirements, req =>
        {
            req.ToTable("role_requirements"); 

            req.WithOwner().HasForeignKey("role_id");
            
            req.Property(r => r.ValueName)
                .HasMaxLength(128)
                .IsRequired();

            req.HasKey("role_id", "ValueName"); 
        });
    }
}