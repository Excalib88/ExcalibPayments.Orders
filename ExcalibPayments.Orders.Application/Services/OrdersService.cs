using ExcalibPayments.Orders.Application.Abstractions;
using ExcalibPayments.Orders.Application.Mappers;
using ExcalibPayments.Orders.Application.Models.Orders;
using ExcalibPayments.Orders.Domain;
using ExcalibPayments.Orders.Domain.Entities;
using ExcalibPayments.Orders.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExcalibPayments.Orders.Application.Services;

public class OrdersService(OrdersDbContext context, ICartsService cartsService) : IOrdersService
{
    public async Task<OrderDto> Create(CreateOrderDto order)
    {
        var orderByOrderNumber = await context.Orders.FirstOrDefaultAsync(x => 
            x.OrderNumber == order.OrderNumber && x.MerchantId == order.MerchantId);

        if (orderByOrderNumber != null)
        {
            throw new DuplicateEntityException($"Order with orderNumber {order.OrderNumber} is exist for merchant " +
                                               $"{order.MerchantId}");
        }
        
        if (order.Cart == null)
        {
            throw new ArgumentNullException();
        }
        
        var cart = await cartsService.Create(order.Cart);
        var entity = new OrderEntity
        {
            OrderNumber = order.OrderNumber,
            Name = order.Name,
            CustomerId = order.CustomerId,
            CartId = cart.Id
        };

        var orderSaveResult = await context.Orders.AddAsync(entity);
        await context.SaveChangesAsync();

        var orderEntityResult = orderSaveResult.Entity;

        return orderEntityResult.ToDto();
    }

    public async Task<OrderDto> GetById(long orderId)
    {
        var entity = await context.Orders
            .Include(o => o.Cart)
            .ThenInclude(c => c.CartItems)
            .FirstOrDefaultAsync(x => x.Id == orderId);

        if (entity == null)
        {
            throw new EntityNotFoundException($"Order entity with id {orderId} not found");
        }

        return entity.ToDto();
    }

    public async Task<List<OrderDto>> GetByUser(long customerId)
    {
        var entity = await context.Orders
            .Include(o => o.Cart)
            .ThenInclude(c => c.CartItems)
            .Where(x => x.CustomerId == customerId)
            .ToListAsync();

        return entity.Select(x => x.ToDto()).ToList();
    }

    public async Task<List<OrderDto>> GetAll()
    {
        var entity = await context.Orders
            .Include(o => o.Cart)
            .ThenInclude(c => c.CartItems)
            .ToListAsync();

        return entity.Select(x => x.ToDto()).ToList();
    }

    //todo: прикрутить статусную модель для отмены заказа
    public Task Reject(long orderId)
    {
        throw new NotImplementedException();
    }
}