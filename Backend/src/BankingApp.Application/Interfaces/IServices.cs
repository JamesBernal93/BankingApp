using BankingApp.Application.DTOs;

namespace BankingApp.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}

public interface IBankAccountService
{
    Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request, string userId, CancellationToken ct = default);
    Task<AccountResponse> GetAccountAsync(Guid accountId, string userId, CancellationToken ct = default);
    Task<IEnumerable<AccountResponse>> GetUserAccountsAsync(string userId, CancellationToken ct = default);
    Task<BalanceResponse> GetBalanceAsync(Guid accountId, string userId, CancellationToken ct = default);
    Task<AccountResponse> DepositAsync(DepositRequest request, string userId, CancellationToken ct = default);
}

public interface ITransferService
{
    Task TransferAsync(TransferRequest request, string userId, CancellationToken ct = default);
}

public interface ITransactionService
{
    Task<IEnumerable<TransactionResponse>> GetAccountTransactionsAsync(Guid accountId, string userId, CancellationToken ct = default);
}

public interface IJwtService
{
    string GenerateToken(string userId, string username);
}
