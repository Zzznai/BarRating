using BarRating.Commons;
using BarRating.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarRating.Data.Seeders
{
    public class AdminUserSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var dbContext = new BarRatingDbContext(serviceProvider.GetRequiredService<DbContextOptions<BarRatingDbContext>>()))
            {
                var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(GlobalConstants.AdministratorRoleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(GlobalConstants.AdministratorRoleName));
                }

                if (!await roleManager.RoleExistsAsync(GlobalConstants.UserRoleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(GlobalConstants.UserRoleName));
                }
            }
        }

        public static async Task AddAdminUser(IServiceProvider serviceProvider)
        {
            using (var dbContext = new BarRatingDbContext(serviceProvider.GetRequiredService<DbContextOptions<BarRatingDbContext>>()))
            {
                var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

                var user = await userManager.FindByNameAsync("admin");

                if (user == null)
                {
                    user = new ApplicationUser()
                    {
                        FirstName = "Master",
                        LastName = "Administrator",
                        UserName = "Administrator"
                    };

                    await userManager.CreateAsync(user);

                    await userManager.AddPasswordAsync(user, "Admin2005!");

                    await userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
                }
            }
        }
    }
}
