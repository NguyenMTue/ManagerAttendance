using ManagerAttendance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagerAttendance.Configuration;

public class QAConfiguration : IEntityTypeConfiguration<QA>
{
    public void Configure(EntityTypeBuilder<QA> builder)
    {
        builder.Property(q => q.TestingMethodology)
            .HasMaxLength(200);
    }
}
