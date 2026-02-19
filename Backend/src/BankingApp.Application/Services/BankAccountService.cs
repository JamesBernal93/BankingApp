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

    public async Task<AccountResponse> DepositAsync(DepositRequest request, string userId, CancellationToken ct = default)
    {
        var account = await GetAndValidateOwnership(request.AccountId, userId, ct);
        account.Deposit(request.Amount, request.Description);
        await _uow.Accounts.UpdateAsync(account, ct);
        await _uow.SaveChangesAsync(ct);
        return MapToResponse(account);
    }

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
