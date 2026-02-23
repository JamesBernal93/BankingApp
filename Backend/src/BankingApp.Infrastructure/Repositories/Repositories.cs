using BankingApp.Domain.Entities;
using BankingApp.Domain.Repositories;
using BankingApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BankingApp.Infrastructure.Repositories;

public class BankAccountRepository : IBankAccountRepository
{
    private readonly BankingDbContext _ctx;
    public BankAccountRepository(BankingDbContext ctx) => _ctx = ctx;

    //public async Task<BankAccount?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
    //    await _ctx.BankAccounts.Include(a => a.Transactions).FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<BankAccount?> GetByAccountNumberAsync(string accountNumber, CancellationToken ct = default) =>
        await _ctx.BankAccounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, ct);

    public async Task<IEnumerable<BankAccount>> GetByUserIdAsync(string userId, CancellationToken ct = default) =>
        await _ctx.BankAccounts.Where(a => a.UserId == userId).ToListAsync(ct);

    public async Task<BankAccount> AddAsync(BankAccount account, CancellationToken ct = default)
    {
        await _ctx.BankAccounts.AddAsync(account, ct);
        return account;
    }

    public Task UpdateAsync(BankAccount account, CancellationToken ct = default)
    {
        _ctx.BankAccounts.Update(account);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        await _ctx.BankAccounts.AnyAsync(a => a.Id == id, ct);
    public async Task<BankAccount?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
    await _ctx.BankAccounts
        .Include(a => a.Transactions)
        .FirstOrDefaultAsync(a => a.Id == id, ct);

    // Nuevo método — sin Include, para operaciones de escritura
    public async Task<BankAccount?> GetByIdNoTrackingAsync(Guid id, CancellationToken ct = default) =>
        await _ctx.BankAccounts
            .FirstOrDefaultAsync(a => a.Id == id, ct);
}

public class TransactionRepository : ITransactionRepository
{
    private readonly BankingDbContext _ctx;
    public TransactionRepository(BankingDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default) =>
        await _ctx.Transactions.Where(t => t.AccountId == accountId).OrderByDescending(t => t.CreatedAt).ToListAsync(ct);

    public async Task AddAsync(Transaction transaction, CancellationToken ct = default) =>
        await _ctx.Transactions.AddAsync(transaction, ct);

    public async Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken ct = default) =>
        await _ctx.Transactions.AddRangeAsync(transactions, ct);
}

public class UserRepository : IUserRepository
{
    private readonly BankingDbContext _ctx;
    public UserRepository(BankingDbContext ctx) => _ctx = ctx;

    public async Task<User?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _ctx.Users.FindAsync(new object[] { id }, ct);

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        await _ctx.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await _ctx.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        await _ctx.Users.AddAsync(user, ct);
        return user;
    }
}

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly BankingDbContext _ctx;
    private IDbContextTransaction? _transaction;

    public IBankAccountRepository Accounts { get; }
    public ITransactionRepository Transactions { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(BankingDbContext ctx)
    {
        _ctx = ctx;
        Accounts = new BankAccountRepository(ctx);
        Transactions = new TransactionRepository(ctx);
        Users = new UserRepository(ctx);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _ctx.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default) =>
        _transaction = await _ctx.Database.BeginTransactionAsync(ct);

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            try
            {
                await _transaction.CommitAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    public void DetachEntity<T>(T entity) where T : class
    {
        _ctx.Entry(entity).State = EntityState.Detached;
    }
    public void Dispose() => _ctx.Dispose();
}
