using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Identity.Data;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Seeding
{
    public class IdentityDataSeeder : IDataSeeder
    {
        private readonly StoreIdentityDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityDataSeeder(StoreIdentityDbContext dbContext,ILogger<IdentityDataSeeder> logger,
            UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task SeedAsync(CancellationToken ct = default)
        {
            try
            {
                var PendingMigratons = await _dbContext.Database.GetPendingMigrationsAsync(ct);
                if (PendingMigratons.Any())
                    await _dbContext.Database.MigrateAsync(ct);
                if(!await _roleManager.Roles.AnyAsync(ct))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }
                if (!await _userManager.Users.AnyAsync(ct))
                {
                    var Admin = new ApplicationUser
                    {
                        DisplayName = "Mohammed Ahmed",
                        Email = "Mohammed@Gmail.Com",
                        UserName = "Mohamed",
                        PhoneNumber = "01225770196"
                    };
                    var CreateResult = await _userManager.CreateAsync(Admin, "P@ssw0rd");
                    if (CreateResult.Succeeded)
                    {
                        await _userManager.CreateAsync(Admin, "Admin");
                    }
                    else
                    {
                        _logger.LogWarning("Could Not Seed Default Admin User:{Errors}",
                            string.Join(";", CreateResult.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, "Identity Data Seeding Failed");
            }
        }
    }
}
