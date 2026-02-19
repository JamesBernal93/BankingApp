namespace BankingApp.Domain.Entities;

public enum TransactionType
{
    Credit,
    Debit,
    Transfer
}

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public decimal BalanceAfter { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? RelatedAccountId { get; private set; }

    // EF Core constructor
    private Transaction() { }

    public Transaction(Guid accountId, TransactionType type, decimal amount, decimal balanceAfter, string description, Guid? relatedAccountId = null)
    {
        Id = Guid.NewGuid();
        AccountId = accountId;
        Type = type;
        Amount = amount;
        BalanceAfter = balanceAfter;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        RelatedAccountId = relatedAccountId;
    }
}
