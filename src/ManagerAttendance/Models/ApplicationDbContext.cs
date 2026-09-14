using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManagerAttendance.Models;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Developer> Developers => Set<Developer>();
    public DbSet<QA> QAs => Set<QA>();
    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure TPH (Table-Per-Hierarchy) for Employee
        builder.Entity<Employee>()
            .HasDiscriminator<string>("EmployeeType")
            .HasValue<Developer>("Developer")
            .HasValue<QA>("QA")
            .HasValue<Manager>("Manager");

        builder.Entity<Employee>()
            .Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Entity<AttendanceRecord>()
            .HasOne(a => a.Employee)
            .WithMany(e => e.AttendanceRecords)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
