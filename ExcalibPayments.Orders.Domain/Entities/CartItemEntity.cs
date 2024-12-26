namespace ExcalibPayments.Orders.Domain.Entities;

public class CartItemEntity : BaseEntity
{
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    public CartEntity? Cart { get; set; }
    public long? CartId { get; set; }
}