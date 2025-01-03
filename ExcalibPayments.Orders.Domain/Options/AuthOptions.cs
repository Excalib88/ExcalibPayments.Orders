namespace ExcalibPayments.Orders.Domain.Options;

public class AuthOptions
{
    public required string TokenPrivateKey { get; set; }
    public int ExpireIntervalMinutes { get; set; }
}