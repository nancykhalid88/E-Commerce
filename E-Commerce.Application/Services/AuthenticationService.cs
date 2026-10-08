using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IIdentityService identityService,ITokenService tokenService)
        {
           _identityService = identityService;
           _tokenService = tokenService;
        }

        public async Task<Result<bool>> CheckEmailAsync(string email, CancellationToken ct = default)
        => await _identityService.EmailExistsAsync(email,ct);


        public async Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken ct = default)
        {
            var result = await _identityService.FindByEmailAsync(email, ct);
            if (!result.IsSuccess)
                return Result<UserDto>.Fail(result.Errors);
            var user = result.data;
            var rolesResult = await _identityService.GetRolesAsync(email, ct);
            if (!rolesResult.IsSuccess)
                return Result<UserDto>.Fail(rolesResult.Errors);
            var roles = rolesResult.data;
            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token=token
            };
        }


        public async Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken ct = default)
        {
            var result = await _identityService.GetAddressByEmailAsync(email, ct);
            if (!result.IsSuccess)
                return Result<AddressDto>.Fail(result.Errors);
            return result.data;
        }


        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            //get user by email
            var UserResult = await _identityService.FindByEmailAsync(loginDto.Email);
            if (!UserResult.IsSuccess)
                return Result<UserDto>.Fail(UserResult.Errors);
            //Check password
            var PasswordResult = await _identityService.CheckPasswordAsync(loginDto.Email,loginDto.Password);
            if (!PasswordResult.IsSuccess)
                return Result<UserDto>.Fail(Error.Unauthorized("Invalid Email Or Password"));
            var rolesResult = await _identityService.GetRolesAsync(loginDto.Email, ct);
            if (!rolesResult.IsSuccess)
                return Result<UserDto>.Fail(rolesResult.Errors);
            var roles = rolesResult.data;
            var user = UserResult.data;
            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = token
            };
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var result = await _identityService.CreateUserAsync(registerDto, ct);
            if (!result.IsSuccess || result.data is null)
            {
                return Result<UserDto>.Fail(result.Errors);
            }
            return new UserDto
            {
                Email = result.data.Email,
                DisplayName = result.data.DisplayName,
                Token = "Token"
            };
        }

        public async Task<Result<AddressDto>> UpdateUserAddressAsync(AddressDto addressDto, string email, CancellationToken ct = default)
        => await _identityService.UpdateAddressAsync(email,addressDto, ct); 
    }
}
