using BankingApp.Application.DTOs;
using BankingApp.Application.Interfaces;
using BankingApp.Domain.Exceptions;
using BankingApp.Domain.Repositories;
using BankingApp.Domain.Services;
using System.Security.Principal;

namespace BankingApp.Application.Services;

public class TransferService : ITransferService
{
    private readonly IUnitOfWork _uow;
    private readonly ITransferDomainService _transferDomainService;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public TransferService(IUnitOfWork uow, ITransferDomainService transferDomainService)
    {
        _uow = uow;
        _transferDomainService = transferDomainService;
    }

    public async Task TransferAsync(TransferRequest request, string userId, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            await _uow.BeginTransactionAsync(ct);

            var source = await _uow.Accounts.GetByIdAsync(request.SourceAccountId, ct)
                ?? throw new AccountNotFoundException(request.SourceAccountId);

            if (source.UserId != userId)
                throw new UnauthorizedAccountAccessException();

            var destination = await _uow.Accounts.GetByAccountNumberAsync(request.DestinationAccountNumber, ct)
                ?? throw new AccountNotFoundException(request.DestinationAccountNumber);

            _transferDomainService.Transfer(source, destination, request.Amount, request.Description);

            await _uow.Accounts.UpdateAsync(source, ct);
            await _uow.Transactions.AddAsync(source.Transactions.OrderByDescending(c => c.CreatedAt).FirstOrDefault());

            await _uow.Accounts.UpdateAsync(destination, ct);
            await _uow.Transactions.AddAsync(destination.Transactions.OrderByDescending(c => c.CreatedAt).FirstOrDefault());

            try
            {
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);

            }
            catch (Exception ex)
            {

                throw new AccountNotFoundException(request.DestinationAccountNumber);
            }
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
