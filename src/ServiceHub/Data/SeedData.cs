using Microsoft.AspNetCore.Identity;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        //var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure admin role exists
        //const string adminRole = "Admin";
        //if (!await roleManager.RoleExistsAsync(adminRole))
        //{
        //    await roleManager.CreateAsync(new IdentityRole(adminRole));
        //}

        // Ensure admin user exists
        const string adminEmail = "admin@Datatyr.com";
        const string adminPassword = "P@ssw0rd";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            //if (result.Succeeded)
            //{
            //    await userManager.AddToRoleAsync(adminUser, adminRole);
            //}
        }
    }
}
