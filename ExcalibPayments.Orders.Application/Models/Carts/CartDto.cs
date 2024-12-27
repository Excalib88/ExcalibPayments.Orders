namespace ExcalibPayments.Orders.Application.Models.Carts;

public class CartDto
{
    public long? Id { get; set; }
    public List<CartItemDto> CartItems { get; set; } = null!;
}