namespace ExcalibPayments.Orders.Application.Models.Merchants;

public class MerchantDto
{
    public long? Id { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public string? WebSite { get; set; }
}