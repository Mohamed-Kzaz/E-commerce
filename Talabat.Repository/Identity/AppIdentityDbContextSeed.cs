using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Mohamed Tarek",
                    Email = "mohamed.tarek@gmail.com",
                    UserName = "mohamed.tarek",
                    PhoneNumber = "0151949232"
                };

                await userManager.CreateAsync(user, "Pass1234");
            }
        }
    }
}
