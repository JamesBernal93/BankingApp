namespace BankingApp.Application.DTOs;

// Auth DTOs
public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Username, string Password);
public record AuthResponse(string Token, string UserId, string Username, DateTime ExpiresAt);

// Account DTOs
public record CreateAccountRequest(string OwnerName, string Email, decimal InitialBalance);
public record AccountResponse(
    Guid Id,
    string AccountNumber,
    string OwnerName,
    string Email,
    decimal Balance,
    bool IsActive,
    DateTime CreatedAt);

// Transaction DTOs
public record TransferRequest(Guid SourceAccountId, string DestinationAccountNumber, decimal Amount, string Description);
public record DepositRequest(Guid AccountId, decimal Amount, string Description);
public record TransactionResponse(
    Guid Id,
    Guid AccountId,
    string Type,
    decimal Amount,
    decimal BalanceAfter,
    string Description,
    DateTime CreatedAt,
    Guid? RelatedAccountId);

public record BalanceResponse(Guid AccountId, string AccountNumber, decimal Balance);
