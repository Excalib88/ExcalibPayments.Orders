using ExcalibPayments.Orders.Application.Models.Authentication;

namespace ExcalibPayments.Orders.Application.Abstractions;

public interface IAuthService
{
    Task<UserResponse> Register(UserRegisterDto userRegisterModel);
    Task<UserResponse> Login(UserLoginDto userLoginDto);
}