namespace BankingApp.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class InsufficientFundsException : DomainException
{
    public decimal CurrentBalance { get; }
    public decimal RequestedAmount { get; }

    public InsufficientFundsException(decimal currentBalance, decimal requestedAmount)
        : base($"Insufficient funds. Current balance: {currentBalance:C}, requested: {requestedAmount:C}")
    {
        CurrentBalance = currentBalance;
        RequestedAmount = requestedAmount;
    }
}

public class AccountNotFoundException : DomainException
{
    public AccountNotFoundException(Guid id) : base($"Account with id '{id}' was not found.") { }
    public AccountNotFoundException(string accountNumber) : base($"Account '{accountNumber}' was not found.") { }
}

public class UnauthorizedAccountAccessException : DomainException
{
    public UnauthorizedAccountAccessException() : base("You are not authorized to access this account.") { }
}
