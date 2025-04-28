using Microsoft.AspNetCore.Identity;

namespace Presentation.Services;

internal class SeedDatabase(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    internal async Task SeedRoles(string[] roles)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
