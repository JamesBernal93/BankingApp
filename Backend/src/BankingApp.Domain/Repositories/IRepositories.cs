using BankingApp.Domain.Entities;

namespace BankingApp.Domain.Repositories;

public interface IBankAccountRepository
{
    Task<BankAccount?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BankAccount?> GetByIdNoTrackingAsync(Guid id, CancellationToken ct = default);
    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber, CancellationToken ct = default);
    Task<IEnumerable<BankAccount>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<BankAccount> AddAsync(BankAccount account, CancellationToken ct = default);
    Task UpdateAsync(BankAccount account, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default);
    Task AddAsync(Transaction transaction, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken ct = default);
}

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User> AddAsync(User user, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    IBankAccountRepository Accounts { get; }
    ITransactionRepository Transactions { get; }
    IUserRepository Users { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
    void DetachEntity<T>(T entity) where T : class;
}
