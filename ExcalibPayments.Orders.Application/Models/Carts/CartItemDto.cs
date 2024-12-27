namespace ExcalibPayments.Orders.Application.Models.Carts;

public class CartItemDto
{
    public long? Id { get; set; }
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}