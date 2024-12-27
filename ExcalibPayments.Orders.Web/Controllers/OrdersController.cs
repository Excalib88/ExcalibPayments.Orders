using System.Text.Json;
using ExcalibPayments.Orders.Application.Abstractions;
using ExcalibPayments.Orders.Application.Models.Orders;
using Microsoft.AspNetCore.Mvc;

namespace ExcalibPayments.Orders.Web.Controllers;

[Route("api/orders")]
public class OrdersController(IOrdersService orders, ILogger<OrdersController> logger) : ApiBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto request)
    {
        logger.LogInformation($"Method api/order Create started. Request: {JsonSerializer.Serialize(request)}");
        
        var result = await orders.Create(request);
        
        logger.LogInformation($"Method api/order Create finished. Request: {JsonSerializer.Serialize(request)}" +
                              $"Response: {JsonSerializer.Serialize(result)}");
        
        return Ok(result);
    }
}