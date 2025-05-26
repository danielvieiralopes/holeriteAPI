using HoleriteApi.Models.Enum;
using Microsoft.AspNetCore.Identity;

namespace HoleriteApi.Services;

public static class IdentitySeeder
{
    public static async Task SeedDefaultRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in Enum.GetNames<ERoles>())
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create role '{roleName}': {errors}");
                }
            }
        }
    }
}
