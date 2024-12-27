using ExcalibPayments.Orders.Application.Models.Orders;

namespace ExcalibPayments.Orders.Application.Abstractions;

public interface IOrdersService
{
    Task<OrderDto> Create(CreateOrderDto order);
    Task<OrderDto> GetById(Guid orderId);
    Task<List<OrderDto>> GetByUser(Guid customerId);
    Task<List<OrderDto>> GetAll();
    Task Reject(Guid orderId);
}