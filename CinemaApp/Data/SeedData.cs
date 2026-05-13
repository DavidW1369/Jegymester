using CinemaApp.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CinemaApp.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db          = services.GetRequiredService<AppDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        // Returns true when the DB file was just created (schema didn't exist before).
        // Delete cinema.db and restart to trigger a full re-seed.
        bool freshDb = await db.Database.EnsureCreatedAsync();

        // Ensure all three roles exist.
        foreach (var role in new[] { "User", "Cashier", "Admin" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Default admin account — change credentials before deploying.
        const string adminEmail    = "admin@cinema.com";
        const string adminPassword = "Admin123!";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new AppUser
            {
                UserName    = adminEmail,
                Email       = adminEmail,
                PhoneNumber = CinemaConstants.CinemaPhone,
                Role        = Role.Admin
            };
            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Populate example films, screenings, users and orders only on a fresh DB.
        if (freshDb)
            await SeedTestData.PopulateAsync(db, userManager);
    }
}
