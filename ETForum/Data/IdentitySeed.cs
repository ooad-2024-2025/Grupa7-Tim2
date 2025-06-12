using ETForum.Models;
using Microsoft.AspNetCore.Identity;

namespace ETForum.Data
{
    public static class IdentitySeed
    {
        public static async Task SeedRolesAndAdministrator(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Korisnik>>();

            string roleName = "Administrator";
            string adminEmail = "admin@etf.unsa.ba";
            string password = "Admin123!";

            // Pronađi korisnika
            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new Korisnik
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    ime = "Admin",
                    prezime = "Korisnik",
                    datumRegistracije = DateTime.Now,
                    status = true
                };
                await userManager.CreateAsync(user, password);
            }

            // Dodaj korisnika u rolu ako nije već
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
