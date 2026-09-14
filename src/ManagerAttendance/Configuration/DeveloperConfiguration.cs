using ManagerAttendance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagerAttendance.Configuration;

public class DeveloperConfiguration : IEntityTypeConfiguration<Developer>
{
    public void Configure(EntityTypeBuilder<Developer> builder)
    {
        builder.Property(d => d.TechnicalDirection)
            .HasMaxLength(200);

        builder.Property(d => d.CodingSkillsFlag)
            .HasMaxLength(500);
    }
}
