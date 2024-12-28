using ExcalibPayments.Orders.Application.Abstractions;
using ExcalibPayments.Orders.Application.Mappers;
using ExcalibPayments.Orders.Application.Models.Carts;
using ExcalibPayments.Orders.Domain;
using ExcalibPayments.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExcalibPayments.Orders.Application.Services;

public class CartsService(OrdersDbContext context) : ICartsService
{
    public async Task<CartDto> Create(CartDto cart)
    {
        var cartEntity = new CartEntity();
        var cartSaveResult= await context.Carts.AddAsync(cartEntity);
        await context.SaveChangesAsync();

        var cartItems = cart.CartItems
            .Select(item => new CartItemEntity
            {
                Name = item.Name,
                Price = item.Price,
                Quantity = item.Quantity,
                CartId = cartSaveResult.Entity.Id
            });

        await context.CartItems.AddRangeAsync(cartItems);
        await context.SaveChangesAsync();

        var result = await context.Carts
            .Include(x => x.CartItems)
            .FirstAsync(x => x.Id == cartSaveResult.Entity.Id);

        return result.ToDto();
    }
}