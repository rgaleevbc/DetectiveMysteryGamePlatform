using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DetectiveMysteryGamePlatform.Api.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if we already have the Admin role
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                // Create Admin role
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Check if we already have an admin user
            var adminEmail = "admin@detective.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Create admin user
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    // Add admin role to user
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
} 