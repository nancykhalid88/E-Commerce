using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService _identityService;

        public AuthenticationService(IIdentityService identityService)
        {
           _identityService = identityService;
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
            return new UserDto
            {
                Email = UserResult.data.Email,
                DisplayName = UserResult.data.DisplayName,
                Token = "Token"

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
    }
}
