using System.Text.Json;
using ExcalibPayments.Orders.Application.Abstractions;
using ExcalibPayments.Orders.Application.Models.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExcalibPayments.Orders.Web.Controllers;

[Route("api/orders")]
public class OrdersController(IOrdersService orders, ILogger<OrdersController> logger) : ApiBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto request)
    {
        logger.LogInformation($"Method api/orders Create started. Request: {JsonSerializer.Serialize(request)}");
        
        var result = await orders.Create(request);
        
        logger.LogInformation($"Method api/orders Create finished. Request: {JsonSerializer.Serialize(request)}" +
                              $"Response: {JsonSerializer.Serialize(result)}");
        
        return Ok(result);
    }

    [HttpGet("{orderId:long}")]
    public async Task<IActionResult> GetById(long orderId)
    {
        logger.LogInformation($"Method api/orders/{orderId} started.");

        var result = await orders.GetById(orderId);
        
        logger.LogInformation($"Method api/orders/{orderId} finished. " +
                              $"Response: {JsonSerializer.Serialize(result)}");
        
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("Method api/orders GetAll started.");

        var result = await orders.GetAll();
        
        logger.LogInformation($"Method api/orders GetAll finished. Result count: {result.Count}");
        
        return Ok(new {orders = result});
    }
    
    [HttpGet("customers/{customerId:long}")]
    public async Task<IActionResult> GetByUser(long customerId)
    {
        logger.LogInformation($"Method api/orders/customers/{customerId} GetByUser started.");

        var result = await orders.GetByUser(customerId);
        
        logger.LogInformation($"Method api/orders/customers/{customerId} GetByUser finished. Result count: {result.Count}");
        
        return Ok(new {orders = result});
    }

    [HttpPost("{orderId:long}/reject")]
    public async Task<IActionResult> Reject(long orderId)
    {
        await orders.Reject(orderId);
        
        return Ok();
    }
}