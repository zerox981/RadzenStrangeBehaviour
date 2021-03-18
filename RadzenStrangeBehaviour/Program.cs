using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RadzenStrangeBehaviour.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RadzenStrangeBehaviour
{
    public class Program
    {
        private static async Task SeedAuthDB(IServiceScope scope)
        {
            try
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var administratorRole = new IdentityRole("admin");
                var seedRoles = new IdentityRole[] {
                    administratorRole,
                    new("user")
                };
                foreach (var role in seedRoles)
                {
                    if (!roleManager.Roles.Any(r => r.Name == role.Name))
                    {
                        await roleManager.CreateAsync(role);
                    }
                }

                var administrator = new IdentityUser { UserName = "admin@local", Email = "admin@local", EmailConfirmed = true };

                if (userManager.Users.All(u => u.UserName != administrator.UserName))
                {
                    var result = await userManager.CreateAsync(administrator, "Changeme1!");
                    if (!result.Succeeded)
                        throw new Exception($"Create user faled! {result}");
                    await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error {ex}");
            }
        }

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                await SeedAuthDB(scope);
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}