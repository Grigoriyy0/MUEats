using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Infrastructure.Persistence.Configurations;

public class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.HasOne(x => x.Role)
            .WithMany(y => y.Roles)
            .HasForeignKey(z => z.RoleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.User)
            .WithMany(y => y.UserRoles)
            .HasForeignKey(z => z.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}