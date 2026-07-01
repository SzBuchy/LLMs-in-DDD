using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AppIdentityDbContextSeed
{
    public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var adminRole = "Administrators";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var adminUserName = "admin@microsoft.com";
        var adminUser = await userManager.FindByNameAsync(adminUserName);
        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = adminUserName, Email = adminUserName };
            await userManager.CreateAsync(adminUser, "Pass@word1");
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }

        var demoUserName = "demouser@microsoft.com";
        var demoUser = await userManager.FindByNameAsync(demoUserName);
        if (demoUser == null)
        {
            demoUser = new IdentityUser { UserName = demoUserName, Email = demoUserName };
            await userManager.CreateAsync(demoUser, "Pass@word1");
        }
    }
}
