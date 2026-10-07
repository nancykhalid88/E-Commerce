using E_Commerce.Application.Common;
using E_Commerce.Application.DTOS.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IIdentityService
    {
        public Task<Result<IdentityUserResult>> FindByEmailAsync(string email, CancellationToken ct=default);
        public Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default);
        public Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default);
    }
}
