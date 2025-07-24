// DomainExceptions.cs
namespace VendingMachine.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(string userId) : base($"User with ID '{userId}' was not found.") { }
}

public class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(int productId) : base($"Product with ID '{productId}' was not found.") { }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message) : base(message) { }
}

public class InsufficientFundsException : DomainException
{
    public InsufficientFundsException(int required, int available)
        : base($"Insufficient funds. Required: {required} cents, Available: {available} cents.") { }
}

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string productName, int requested, int available)
        : base($"Insufficient stock for '{productName}'. Requested: {requested}, Available: {available}.") { }
}

public class InvalidCoinException : DomainException
{
    public InvalidCoinException(int coinValue)
        : base($"Invalid coin denomination: {coinValue}. Valid denominations are: 5, 10, 20, 50, 100 cents.") { }
}

public class InvalidRoleException : DomainException
{
    public InvalidRoleException(string action, string requiredRole)
        : base($"Action '{action}' requires '{requiredRole}' role.") { }
}

public class DuplicateUsernameException : DomainException
{
    public DuplicateUsernameException(string username)
        : base($"Username '{username}' is already taken.") { }
}