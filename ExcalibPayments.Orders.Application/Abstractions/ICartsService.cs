using ExcalibPayments.Orders.Application.Models.Carts;

namespace ExcalibPayments.Orders.Application.Abstractions;

public interface ICartsService
{
    Task<CartDto> Create(CartDto cart);
}