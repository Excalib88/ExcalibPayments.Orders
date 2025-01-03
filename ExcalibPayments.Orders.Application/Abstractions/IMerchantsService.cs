using ExcalibPayments.Orders.Application.Models.Merchants;
using ExcalibPayments.Orders.Domain.Entities;

namespace ExcalibPayments.Orders.Application.Abstractions;

public interface IMerchantsService
{
    Task<MerchantDto> Create(MerchantDto merchant);
}