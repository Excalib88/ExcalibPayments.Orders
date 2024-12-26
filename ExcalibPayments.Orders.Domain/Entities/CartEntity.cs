namespace ExcalibPayments.Orders.Domain.Entities;

public class CartEntity : BaseEntity
{
    public List<CartItemEntity>? CartItems { get; set; }
}