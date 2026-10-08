using E_Commerce.Application.Common;
using E_Commerce.Application.DTOS.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IAuthenticationService
    {
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default);
        Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default);
        Task<Result<bool>> CheckEmailAsync(string email, CancellationToken ct = default);
        Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken ct = default);
        Task<Result<AddressDto>> UpdateUserAddressAsync(AddressDto addressDto, string email, CancellationToken ct = default);
        Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken ct = default);
        
    }
}
