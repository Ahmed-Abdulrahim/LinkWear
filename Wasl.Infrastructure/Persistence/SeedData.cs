namespace Wasl.Infrastructure.Persistence
{
    public static class SeedData
    {
        public static async Task SeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Users
            await SeedUsersAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            var roles = new[]
         {
              new { Name = "Admin", Description = "Full system access and management" },
              new { Name = "StoreOwner", Description = "Place orders, select products, track order status" },
              new { Name = "Supplier", Description = "Receive orders, update status and tracking, contact shipping offline" },
         };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = role.Name,
                        Description = role.Description
                    });
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var users = new[]
            {
        new
        {
            UserName="xyzz235",

            Email = "admin@wasl.com",
            FullName = "System Admin",
            Role = "Admin",
            Password = "P@ssw0rd",
            UserType = UserType.Admin,
            ApprovalStatus = ApprovalStatus.Approved,
            BusinessName = (string?)null,
            PhoneNumber = (string?)null,
            City = (string?)null,
            BusinessDescription = (string?)null,
            ActivityType = ActivityType.Fabrics,
        },
        new
        {
            UserName="abcd123",

            Email = "storeowner@wasl.com",

            FullName = "Store Owner",
            Role = "StoreOwner",
            Password = "P@ssw0rd",
            UserType = UserType.StoreOwner,
            ApprovalStatus = ApprovalStatus.Approved,
            BusinessName = (string?)null,
            PhoneNumber = (string?)null,
            City = (string?)null,
            BusinessDescription = (string?)null,
            ActivityType = ActivityType.Fabrics,
        },
        new
        {
            UserName="cvbn478",

            Email = "supplier@wasl.com",
            FullName = "Test Supplier",

            Role = "Supplier",
            Password = "P@ssw0rd",
            UserType = UserType.Supplier,
            ApprovalStatus = ApprovalStatus.Approved,
            BusinessName = (string?)"مصنع النسيج",
            PhoneNumber = (string?)"01000000000",
            City = (string?)"القاهرة",
            BusinessDescription = (string?)"مصنع متخصص في الأقمشة والملابس",
            ActivityType = ActivityType.Fabrics,
        },
    };

            foreach (var user in users)
            {
                var existingUser = await userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        EmailConfirmed = true,
                        IsDeleted = false,
                        IsActive = true,
                        UserType = user.UserType,
                        ApprovalStatus = user.ApprovalStatus,
                        BusinessName = user.BusinessName,
                        PhoneNumber = user.PhoneNumber,
                        City = user.City,
                        BusinessDescription = user.BusinessDescription,
                        ActivityType = user.ActivityType,
                        CreatedAt = DateTime.UtcNow,
                    };

                    var result = await userManager.CreateAsync(newUser, user.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, user.Role);
                    }
                }
            }
        }
    }
}
