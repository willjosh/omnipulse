using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Configs;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "749b52f3-45b7-4613-bfa6-1fd13790ef01",
                Name = "FleetManager",
                NormalizedName = "FLEETMANAGER"
            },
            new IdentityRole
            {
                Id = "996d88fd-4d3b-4920-a4ad-40ab4b941b04",
                Name = "Technician",
                NormalizedName = "TECHNICIAN"
            }
        );
    }
}
