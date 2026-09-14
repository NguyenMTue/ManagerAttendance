using ManagerAttendance.Models;
using Microsoft.AspNetCore.Identity;

namespace ManagerAttendance;

public static class DatabaseSeeder
{
    public static async Task SeedDataAsync(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Skeleton seeder method: Seed Roles, Default Users, Domain Data
        await Task.CompletedTask;
    }
}
