using BankingApp.Application.DTOs;
using BankingApp.Application.Services;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Exceptions;
using BankingApp.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace BankingApp.Application.Tests;

public class BankAccountServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IBankAccountRepository> _accountRepoMock = new();
    private readonly BankAccountService _service;
    private const string UserId = "user-test-123";

    public BankAccountServiceTests()
    {
        _uowMock.Setup(u => u.Accounts).Returns(_accountRepoMock.Object);
        _service = new BankAccountService(_uowMock.Object);
    }

    [Fact]
    public async Task CreateAccountAsync_WithValidData_ShouldReturnAccountResponse()
    {
        var request = new CreateAccountRequest("Jane Doe", "jane@test.com", 500m);
        BankAccount? captured = null;

        _accountRepoMock.Setup(r => r.AddAsync(It.IsAny<BankAccount>(), default))
            .Callback<BankAccount, CancellationToken>((a, _) => captured = a)
            .ReturnsAsync((BankAccount a, CancellationToken _) => a);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _service.CreateAccountAsync(request, UserId);

        result.Should().NotBeNull();
        result.OwnerName.Should().Be("Jane Doe");
        result.Balance.Should().Be(500m);
        captured!.UserId.Should().Be(UserId);
    }

    [Fact]
    public async Task GetAccountAsync_AccountNotFound_ShouldThrowAccountNotFoundException()
    {
        var id = Guid.NewGuid();
        _accountRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((BankAccount?)null);

        var act = async () => await _service.GetAccountAsync(id, UserId);

        await act.Should().ThrowAsync<AccountNotFoundException>();
    }

    [Fact]
    public async Task GetAccountAsync_WrongOwner_ShouldThrowUnauthorizedException()
    {
        var account = new BankAccount("Owner", "owner@test.com", 100m, "other-user");
        _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id, default)).ReturnsAsync(account);

        var act = async () => await _service.GetAccountAsync(account.Id, UserId);

        await act.Should().ThrowAsync<UnauthorizedAccountAccessException>();
    }

    [Fact]
    public async Task GetBalanceAsync_ValidAccount_ShouldReturnBalance()
    {
        var account = new BankAccount("Owner", "owner@test.com", 750m, UserId);
        _accountRepoMock.Setup(r => r.GetByIdAsync(account.Id, default)).ReturnsAsync(account);

        var result = await _service.GetBalanceAsync(account.Id, UserId);

        result.Balance.Should().Be(750m);
        result.AccountNumber.Should().Be(account.AccountNumber);
    }
}
