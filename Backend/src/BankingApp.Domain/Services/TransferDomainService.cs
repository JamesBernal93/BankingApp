using BankingApp.Domain.Entities;
using BankingApp.Domain.Exceptions;

namespace BankingApp.Domain.Services;

public interface ITransferDomainService
{
    void Transfer(BankAccount source, BankAccount destination, decimal amount, string description);
}

public class TransferDomainService : ITransferDomainService
{
    private const decimal MaxTransferAmount = 1_000_000m;
    private const decimal MinTransferAmount = 0.01m;

    public void Transfer(BankAccount source, BankAccount destination, decimal amount, string description)
    {
        if (source.Id == destination.Id)
            throw new DomainException("Cannot transfer to the same account.");
        if (amount < MinTransferAmount)
            throw new DomainException($"Transfer amount must be at least {MinTransferAmount:C}.");
        if (amount > MaxTransferAmount)
            throw new DomainException($"Transfer amount cannot exceed {MaxTransferAmount:C}.");

        source.Withdraw(amount, $"Transfer to {destination.AccountNumber}: {description}");
        destination.Deposit(amount, $"Transfer from {source.AccountNumber}: {description}");
    }
}
