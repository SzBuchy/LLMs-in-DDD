namespace ApplicationCore.Exceptions;

public class OrderDomainException : Exception
{
    public OrderDomainException(string message) : base(message)
    {
    }
}
