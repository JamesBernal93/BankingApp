using BankingApp.Application.Interfaces;
using BankingApp.Application.Services;
using BankingApp.Domain.Repositories;
using BankingApp.Domain.Services;
using BankingApp.Infrastructure.Data;
using BankingApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankingApp.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<BankingDbContext>(options =>
            options.UseSqlite(config.GetConnectionString("DefaultConnection") ?? "Data Source=banking.db"));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBankAccountService, BankAccountService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITransferDomainService, TransferDomainService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
