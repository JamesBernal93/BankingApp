using BankingApp.Application.DTOs;
using BankingApp.Application.Interfaces;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Exceptions;
using BankingApp.Domain.Repositories;

namespace BankingApp.Application.Services;

public class BankAccountService : IBankAccountService
{
    private readonly IUnitOfWork _uow;

    public BankAccountService(IUnitOfWork uow) => _uow = uow;

    public async Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request, string userId, CancellationToken ct = default)
    {
        var account = new BankAccount(request.OwnerName, request.Email, request.InitialBalance, userId);
        await _uow.Accounts.AddAsync(account, ct);
        await _uow.SaveChangesAsync(ct);
        return MapToResponse(account);
    }

    public async Task<AccountResponse> GetAccountAsync(Guid accountId, string userId, CancellationToken ct = default)
    {
        var account = await GetAndValidateOwnership(accountId, userId, ct);
        return MapToResponse(account);
    }

    public async Task<IEnumerable<AccountResponse>> GetUserAccountsAsync(string userId, CancellationToken ct = default)
    {
        var accounts = await _uow.Accounts.GetByUserIdAsync(userId, ct);
        return accounts.Select(MapToResponse);
    }

    public async Task<BalanceResponse> GetBalanceAsync(Guid accountId, string userId, CancellationToken ct = default)
    {
        var account = await GetAndValidateOwnership(accountId, userId, ct);
        return new BalanceResponse(account.Id, account.AccountNumber, account.Balance);
    }

    //public async Task<AccountResponse> DepositAsync(DepositRequest request, string userId, CancellationToken ct = default)
    //{
    //    // Cargar SIN include de transacciones para evitar el conflicto
    //    var account = await _uow.Accounts.GetByIdAsync(request.AccountId, ct)
    //        ?? throw new AccountNotFoundException(request.AccountId);

    //    if (account.UserId != userId)
    //        throw new UnauthorizedAccountAccessException();

    //    account.Deposit(request.Amount, request.Description);
    //    await _uow.SaveChangesAsync(ct);
    //    return MapToResponse(account);
    //}
    public async Task<AccountResponse> DepositAsync(DepositRequest request, string userId, CancellationToken ct = default)
    {
        var account = await _uow.Accounts.GetByIdAsync(request.AccountId, ct)
            ?? throw new AccountNotFoundException(request.AccountId);

        if (account.UserId != userId)
            throw new UnauthorizedAccountAccessException();
        foreach (var tx in account.Transactions)
        {
            _uow.DetachEntity(tx);
        }

        account.Deposit(request.Amount, request.Description);
        await _uow.Transactions.AddAsync(account.Transactions.OrderByDescending(c=>c.CreatedAt).FirstOrDefault());
        await _uow.SaveChangesAsync(ct);
        return MapToResponse(account);
    }
    //private async Task<BankAccount> GetAndValidateOwnership(Guid accountId, string userId, CancellationToken ct)
    //{
    //    // Usa el método sin Include para operaciones de escritura
    //    var account = await _uow.Accounts.GetByIdNoTrackingAsync(accountId, ct)
    //        ?? throw new AccountNotFoundException(accountId);

    //    if (account.UserId != userId)
    //        throw new UnauthorizedAccountAccessException();

    //    return account;
    //}

    private async Task<BankAccount> GetAndValidateOwnership(Guid accountId, string userId, CancellationToken ct)
    {
        var account = await _uow.Accounts.GetByIdAsync(accountId, ct)
            ?? throw new AccountNotFoundException(accountId);
        if (account.UserId != userId)
            throw new UnauthorizedAccountAccessException();
        return account;
    }

    private static AccountResponse MapToResponse(BankAccount a) =>
        new(a.Id, a.AccountNumber, a.OwnerName, a.Email, a.Balance, a.IsActive, a.CreatedAt);

}
