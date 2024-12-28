using ExcalibPayments.Orders.Application.Models.Carts;
using ExcalibPayments.Orders.Domain.Entities;

namespace ExcalibPayments.Orders.Application.Mappers;

public static class CartsMapper
{
    public static CartDto ToDto(this CartEntity entity)
    {
        return new CartDto
        {
            Id = entity.Id,
            CartItems = entity.CartItems!.Select(item => new CartItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList()
        };
    }

    public static CartEntity ToEntity(this CartDto dto)
    {
        return new CartEntity
        {
            CartItems = dto.CartItems.Select(cart => cart.ToEntity()).ToList()
        };
    }

    public static CartItemEntity ToEntity(this CartItemDto dto)
    {
        return new CartItemEntity
        {
            Name = dto.Name,
            Price = dto.Price,
            Quantity = dto.Quantity
        };
    }
}