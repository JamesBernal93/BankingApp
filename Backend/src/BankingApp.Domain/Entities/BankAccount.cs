using BankingApp.Domain.Exceptions;

namespace BankingApp.Domain.Entities;

public class BankAccount
{
    public Guid Id { get; private set; }
    public string AccountNumber { get; private set; }
    public string OwnerName { get; private set; }
    public string Email { get; private set; }
    public decimal Balance { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string UserId { get; private set; }

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    // EF Core constructor
    private BankAccount() { }

    public BankAccount(string ownerName, string email, decimal initialBalance, string userId)
    {
        if (string.IsNullOrWhiteSpace(ownerName))
            throw new DomainException("Owner name cannot be empty.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");
        if (initialBalance < 0)
            throw new DomainException("Initial balance cannot be negative.");

        Id = Guid.NewGuid();
        AccountNumber = GenerateAccountNumber();
        OwnerName = ownerName;
        Email = email;
        Balance = initialBalance;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UserId = userId;
    }

    public void Deposit(decimal amount, string description = "Deposit")
    {
        if (!IsActive)
            throw new DomainException("Account is not active.");
        if (amount <= 0)
            throw new DomainException("Deposit amount must be greater than zero.");

        Balance += amount;
        UpdatedAt = DateTime.UtcNow;

        _transactions.Add(new Transaction(Id, TransactionType.Credit, amount, Balance, description));
    }

    public void Withdraw(decimal amount, string description = "Withdrawal")
    {
        if (!IsActive)
            throw new DomainException("Account is not active.");
        if (amount <= 0)
            throw new DomainException("Withdrawal amount must be greater than zero.");
        if (amount > Balance)
            throw new InsufficientFundsException(Balance, amount);

        Balance -= amount;
        UpdatedAt = DateTime.UtcNow;

        _transactions.Add(new Transaction(Id, TransactionType.Debit, amount, Balance, description));
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Account is already inactive.");
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateAccountNumber()
    {
        var random = new Random();
        return $"ACC{random.Next(100000000, 999999999)}";
    }
}
