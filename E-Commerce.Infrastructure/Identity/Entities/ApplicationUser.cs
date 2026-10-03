
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Infrastructure.Identity.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string DisplayName { get; set; } = default!;
        public Address Address { get; set; }
    }
}
