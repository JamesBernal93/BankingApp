using BankingApp.Domain.Entities;
using BankingApp.Domain.Exceptions;
using BankingApp.Domain.Services;
using FluentAssertions;
using Xunit;

namespace BankingApp.Domain.Tests;

public class BankAccountTests
{
    private const string UserId = "user-123";

    [Fact]
    public void Constructor_WithValidData_ShouldCreateAccount()
    {
        var account = new BankAccount("John Doe", "john@example.com", 1000m, UserId);

        account.OwnerName.Should().Be("John Doe");
        account.Email.Should().Be("john@example.com");
        account.Balance.Should().Be(1000m);
        account.IsActive.Should().BeTrue();
        account.AccountNumber.Should().StartWith("ACC");
    }

    [Fact]
    public void Constructor_WithNegativeBalance_ShouldThrowDomainException()
    {
        var act = () => new BankAccount("John", "john@example.com", -100m, UserId);

        act.Should().Throw<DomainException>().WithMessage("*negative*");
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowDomainException()
    {
        var act = () => new BankAccount("", "john@example.com", 100m, UserId);

        act.Should().Throw<DomainException>().WithMessage("*Owner name*");
    }

    [Fact]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        var account = new BankAccount("John", "john@example.com", 500m, UserId);

        account.Deposit(300m, "Test deposit");

        account.Balance.Should().Be(800m);
    }

    [Fact]
    public void Deposit_WithZeroAmount_ShouldThrowDomainException()
    {
        var account = new BankAccount("John", "john@example.com", 500m, UserId);

        var act = () => account.Deposit(0m);

        act.Should().Throw<DomainException>().WithMessage("*greater than zero*");
    }

    [Fact]
    public void Withdraw_WithSufficientBalance_ShouldDecreaseBalance()
    {
        var account = new BankAccount("John", "john@example.com", 1000m, UserId);

        account.Withdraw(400m);

        account.Balance.Should().Be(600m);
    }

    [Fact]
    public void Withdraw_WithInsufficientBalance_ShouldThrowInsufficientFundsException()
    {
        var account = new BankAccount("John", "john@example.com", 100m, UserId);

        var act = () => account.Withdraw(200m);

        act.Should().Throw<InsufficientFundsException>();
    }

    [Fact]
    public void Withdraw_OnInactiveAccount_ShouldThrowDomainException()
    {
        var account = new BankAccount("John", "john@example.com", 1000m, UserId);
        account.Deactivate();

        var act = () => account.Withdraw(100m);

        act.Should().Throw<DomainException>().WithMessage("*not active*");
    }

    [Fact]
    public void Deposit_CreatesTransactionRecord()
    {
        var account = new BankAccount("John", "john@example.com", 0m, UserId);

        account.Deposit(100m, "Test");

        account.Transactions.Should().HaveCount(1);
        account.Transactions.First().Type.Should().Be(TransactionType.Credit);
        account.Transactions.First().Amount.Should().Be(100m);
    }

    [Fact]
    public void Deactivate_AlreadyInactiveAccount_ShouldThrowDomainException()
    {
        var account = new BankAccount("John", "john@example.com", 0m, UserId);
        account.Deactivate();

        var act = () => account.Deactivate();

        act.Should().Throw<DomainException>().WithMessage("*already inactive*");
    }
}

public class TransferDomainServiceTests
{
    private readonly TransferDomainService _service = new();

    [Fact]
    public void Transfer_WithValidAccounts_ShouldMoveMoney()
    {
        var source = new BankAccount("Alice", "alice@test.com", 1000m, "user1");
        var dest = new BankAccount("Bob", "bob@test.com", 0m, "user2");

        _service.Transfer(source, dest, 500m, "Test transfer");

        source.Balance.Should().Be(500m);
        dest.Balance.Should().Be(500m);
    }

    [Fact]
    public void Transfer_ToSameAccount_ShouldThrowDomainException()
    {
        var account = new BankAccount("Alice", "alice@test.com", 1000m, "user1");

        var act = () => _service.Transfer(account, account, 100m, "Self");

        act.Should().Throw<DomainException>().WithMessage("*same account*");
    }

    [Fact]
    public void Transfer_AboveMaxAmount_ShouldThrowDomainException()
    {
        var source = new BankAccount("Alice", "alice@test.com", 2_000_000m, "user1");
        var dest = new BankAccount("Bob", "bob@test.com", 0m, "user2");

        var act = () => _service.Transfer(source, dest, 1_500_000m, "Large");

        act.Should().Throw<DomainException>().WithMessage("*cannot exceed*");
    }

    [Fact]
    public void Transfer_WithInsufficientFunds_ShouldThrowInsufficientFundsException()
    {
        var source = new BankAccount("Alice", "alice@test.com", 100m, "user1");
        var dest = new BankAccount("Bob", "bob@test.com", 0m, "user2");

        var act = () => _service.Transfer(source, dest, 500m, "Test");

        act.Should().Throw<InsufficientFundsException>();
    }
}
