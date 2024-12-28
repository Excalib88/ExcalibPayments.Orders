namespace ExcalibPayments.Orders.Domain.Exceptions;

public class EntityNotFoundException(string message) : Exception(message)
{
}