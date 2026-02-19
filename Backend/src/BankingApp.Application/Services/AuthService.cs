using BankingApp.Application.DTOs;
using BankingApp.Application.Interfaces;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Exceptions;
using BankingApp.Domain.Repositories;

namespace BankingApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork uow, IJwtService jwtService)
    {
        _uow = uow;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existing = await _uow.Users.GetByUsernameAsync(request.Username, ct);
        if (existing != null)
            throw new DomainException("Username already exists.");

        var existingEmail = await _uow.Users.GetByEmailAsync(request.Email, ct);
        if (existingEmail != null)
            throw new DomainException("Email already registered.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Username, request.Email, passwordHash);
        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var token = _jwtService.GenerateToken(user.Id, user.Username);
        return new AuthResponse(token, user.Id, user.Username, DateTime.UtcNow.AddHours(24));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByUsernameAsync(request.Username, ct);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Invalid username or password.");

        var token = _jwtService.GenerateToken(user.Id, user.Username);
        return new AuthResponse(token, user.Id, user.Username, DateTime.UtcNow.AddHours(24));
    }
}
