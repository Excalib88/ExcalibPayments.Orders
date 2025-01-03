namespace ExcalibPayments.Orders.Application.Models.Authentication;

public record UserLoginDto(string Username, string Email, string Phone, string Password);