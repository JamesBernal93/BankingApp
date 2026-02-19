using System.Security.Claims;
using BankingApp.Application.DTOs;
using BankingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IBankAccountService _accountService;
    private readonly ITransactionService _transactionService;

    public AccountsController(IBankAccountService accountService, ITransactionService transactionService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    /// <summary>Create a new bank account</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request, CancellationToken ct)
    {
        var account = await _accountService.CreateAccountAsync(request, UserId, ct);
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    /// <summary>Get all accounts for current user</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountResponse>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var accounts = await _accountService.GetUserAccountsAsync(UserId, ct);
        return Ok(accounts);
    }

    /// <summary>Get account by ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccountResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var account = await _accountService.GetAccountAsync(id, UserId, ct);
        return Ok(account);
    }

    /// <summary>Get account balance</summary>
    [HttpGet("{id:guid}/balance")]
    [ProducesResponseType(typeof(BalanceResponse), 200)]
    public async Task<IActionResult> GetBalance(Guid id, CancellationToken ct)
    {
        var balance = await _accountService.GetBalanceAsync(id, UserId, ct);
        return Ok(balance);
    }

    /// <summary>Deposit money into account</summary>
    [HttpPost("{id:guid}/deposit")]
    [ProducesResponseType(typeof(AccountResponse), 200)]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] DepositRequest request, CancellationToken ct)
    {
        var result = await _accountService.DepositAsync(request with { AccountId = id }, UserId, ct);
        return Ok(result);
    }

    /// <summary>Get transaction history for account</summary>
    [HttpGet("{id:guid}/transactions")]
    [ProducesResponseType(typeof(IEnumerable<TransactionResponse>), 200)]
    public async Task<IActionResult> GetTransactions(Guid id, CancellationToken ct)
    {
        var transactions = await _transactionService.GetAccountTransactionsAsync(id, UserId, ct);
        return Ok(transactions);
    }
}
