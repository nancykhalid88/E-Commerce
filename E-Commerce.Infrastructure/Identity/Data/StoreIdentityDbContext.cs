using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity.Data
{
    public class StoreIdentityDbContext(DbContextOptions<StoreIdentityDbContext> options):IdentityDbContext<ApplicationUser>(options)
    {
        #region MyRegion

        #endregion
    }
}
