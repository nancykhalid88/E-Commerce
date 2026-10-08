using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Authentication;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result<bool>.Fail(Error.NotFound("User Not Found"));
            var IsValid = await _userManager.CheckPasswordAsync(user, password);
            return IsValid;
        }

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,
                DisplayName = registerDto.DisplayName
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(errors);
            }
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.Email, user.UserName, user.DisplayName));
        }

        public async Task<Result<bool>> EmailExistsAsync(string email, CancellationToken ct = default)
        => await _userManager.FindByEmailAsync(email) is not null;

        public async Task<Result<IdentityUserResult>> FindByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result<IdentityUserResult>.Fail(Error.NotFound("User Not Found"));
            else
                return new IdentityUserResult(user.Id, user.Email, user.UserName, user.DisplayName);
        }

        public async Task<Result<AddressDto>> GetAddressByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.Users.Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email, ct);
            if(user is null)
                return Result<AddressDto>.Fail(Error.NotFound($"User, {email} not found"));
            if(user?.Address is null)
                return Result<AddressDto>.Fail(Error.NotFound($"Address not found"));
            return new AddressDto
            {
                FirstName = user.Address.FirstName,
                LastName = user.Address.LastName,
                City = user.Address.City,
                Street = user.Address.Street,
                Country = user.Address.Country
            };
        }

        public async Task<Result<IReadOnlyList<string>>> GetRolesAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result<IReadOnlyList<string>>.Fail(Error.NotFound($"User, {email} not found"));
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<Result<AddressDto>> UpdateAddressAsync(string email,AddressDto addressDto, CancellationToken ct = default)
        {
            var user = await _userManager.Users.Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == email, ct);
            if (user is null)
                return Result<AddressDto>.Fail(Error.NotFound($"User, {email} not found"));
            if(user.Address is null)
            {
                user.Address = new Address
                {
                    FirstName = addressDto.FirstName,
                    LastName = addressDto.LastName,
                    City = addressDto.City,
                    Street = addressDto.Street,
                    Country = addressDto.Country
                };
            }
            else
            {
                user.Address.FirstName = addressDto.FirstName;
                user.Address.LastName = addressDto.LastName;
                user.Address.City = addressDto.City;
                user.Address.Country = addressDto.Country;
                user.Address.Street = addressDto.Street;
            }
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Result<AddressDto>.Fail(Error.Failure("Failure", string.Join(";",result.Errors.Select(e=>e.Description))));
            return addressDto;
        }
    }
}
