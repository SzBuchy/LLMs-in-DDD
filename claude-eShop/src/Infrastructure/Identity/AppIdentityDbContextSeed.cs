using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public static class AppIdentityDbContextSeed
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        const string demoUserEmail = "demouser@microsoft.com";
        const string demoUserPassword = "Pass123$";

        if (await userManager.FindByEmailAsync(demoUserEmail) != null)
        {
            return;
        }

        var demoUser = new ApplicationUser
        {
            UserName = demoUserEmail,
            Email = demoUserEmail,
            EmailConfirmed = true,
        };

        await userManager.CreateAsync(demoUser, demoUserPassword);
    }
}
