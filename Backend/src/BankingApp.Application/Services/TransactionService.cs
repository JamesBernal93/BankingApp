using BankingApp.Application.DTOs;
using BankingApp.Application.Interfaces;
using BankingApp.Domain.Exceptions;
using BankingApp.Domain.Repositories;

namespace BankingApp.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _uow;

    public TransactionService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<TransactionResponse>> GetAccountTransactionsAsync(Guid accountId, string userId, CancellationToken ct = default)
    {
        var account = await _uow.Accounts.GetByIdAsync(accountId, ct)
            ?? throw new AccountNotFoundException(accountId);

        if (account.UserId != userId)
            throw new UnauthorizedAccountAccessException();

        var transactions = await _uow.Transactions.GetByAccountIdAsync(accountId, ct);
        return transactions.Select(t => new TransactionResponse(
            t.Id, t.AccountId, t.Type.ToString(), t.Amount, t.BalanceAfter, t.Description, t.CreatedAt, t.RelatedAccountId));
    }
}
