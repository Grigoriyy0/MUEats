using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Infrastructure.Persistence.Configurations;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.RoleName)
            .IsUnique();

        builder.HasMany(x => x.Requirements)
            .WithOne(y => y.Role)
            .HasForeignKey(z => z.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}