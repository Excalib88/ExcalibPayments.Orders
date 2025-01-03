using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using ExcalibPayments.Orders.Application.Abstractions;
using ExcalibPayments.Orders.Application.Models.Authentication;
using ExcalibPayments.Orders.Domain.Entities;
using ExcalibPayments.Orders.Domain.Exceptions;
using ExcalibPayments.Orders.Domain.Models;
using ExcalibPayments.Orders.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ExcalibPayments.Orders.Application.Services;

public class AuthService(IOptions<AuthOptions> authOptions, 
    UserManager<UserEntity> userManager) : IAuthService
{
    private readonly AuthOptions _authOptions = authOptions.Value;
    
    public async Task<UserResponse> Register(UserRegisterDto userRegisterDto)
    {
        if (await userManager.FindByEmailAsync(userRegisterDto.Email) != null)
        {
            throw new DuplicateEntityException($"Email {userRegisterDto.Email} already exists");
        }

        var createUserResult = await userManager.CreateAsync(new UserEntity
        {
            Email = userRegisterDto.Email,
            PhoneNumber = userRegisterDto.Phone,
            UserName = userRegisterDto.Username
        }, userRegisterDto.Password);
        
        if (createUserResult.Succeeded)
        {
            var user = await userManager.FindByEmailAsync(userRegisterDto.Email);

            if (user == null)
            {
                throw new EntityNotFoundException($"User with email {userRegisterDto.Email} not registered");
            }
            
            var result = await userManager.AddToRoleAsync(user, RoleConsts.User);

            if (result.Succeeded)
            {
                var response = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = [RoleConsts.User],
                    Username = user.UserName,
                    Phone = user.PhoneNumber
                };
                return GenerateToken(response);
            }

            throw new Exception($"Errors: {string.Join(";", result.Errors
                .Select(x => $"{x.Code} {x.Description}"))}");
        }

        throw new Exception();
    }

    public async Task<UserResponse> Login(UserLoginDto userLoginDto)
    {
        var user = await userManager.FindByEmailAsync(userLoginDto.Email);

        if (user == null)
        {
            throw new EntityNotFoundException($"User with email {userLoginDto.Email} not found");
        }
        
        var checkPasswordResult = await userManager.CheckPasswordAsync(user, userLoginDto.Password);

        if (checkPasswordResult)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            var response = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Roles = userRoles.ToArray(),
                Username = user.UserName,
                Phone = user.PhoneNumber
            };
            return GenerateToken(response);
        }

        throw new AuthenticationException();
    }

    public UserResponse GenerateToken(UserResponse userRegisterModel)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authOptions.TokenPrivateKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);
        
        var claims = new Dictionary<string, object>
        {
            {ClaimTypes.Name, userRegisterModel.Email!},
            {ClaimTypes.NameIdentifier, userRegisterModel.Id.ToString()},
            {JwtRegisteredClaimNames.Aud, "test"},
            {JwtRegisteredClaimNames.Iss, "test"}
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(userRegisterModel),
            Expires = DateTime.UtcNow.AddMinutes(_authOptions.ExpireIntervalMinutes),
            SigningCredentials = credentials,
            Claims = claims,
            Audience = "test",
            Issuer = "test"
        };

        var token = handler.CreateToken(tokenDescriptor);
        userRegisterModel.Token = handler.WriteToken(token);

        return userRegisterModel;
    }

    private static ClaimsIdentity GenerateClaims(UserResponse userRegisterModel)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.Name, userRegisterModel.Email!));
        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, userRegisterModel.Id.ToString()));
        claims.AddClaim(new Claim(JwtRegisteredClaimNames.Aud, "test"));
        claims.AddClaim(new Claim(JwtRegisteredClaimNames.Iss, "test"));

        foreach (var role in userRegisterModel.Roles!)
            claims.AddClaim(new Claim(ClaimTypes.Role, role));

        return claims;
    }
}