using ExcalibPayments.Orders.Application.Abstractions;
using ExcalibPayments.Orders.Application.Models.Merchants;
using ExcalibPayments.Orders.Domain;
using ExcalibPayments.Orders.Domain.Entities;

namespace ExcalibPayments.Orders.Application.Services;

public class MerchantsService(OrdersDbContext context) : IMerchantsService
{
    public async Task<MerchantDto> Create(MerchantDto merchant)
    {
        var entity = new MerchantEntity
        {
            Name = merchant.Name,
            Phone = merchant.Phone,
            WebSite = merchant.WebSite
        };

        var result = await context.Merchants.AddAsync(entity);
        var resultEntity = result.Entity;
        
        await context.SaveChangesAsync();

        return new MerchantDto
        {
            Name = resultEntity.Name,
            Phone = resultEntity.Phone,
            WebSite = resultEntity.WebSite,
            Id = resultEntity.Id
        };
    }
}