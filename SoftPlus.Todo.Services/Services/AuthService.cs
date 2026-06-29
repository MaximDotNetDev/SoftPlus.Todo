using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SoftPlus.Todo.Interfaces.DTOs.Auth;
using SoftPlus.Todo.Interfaces.Entities;
using SoftPlus.Todo.Interfaces.Repositories;
using SoftPlus.Todo.Interfaces.Services;

namespace SoftPlus.Todo.Services.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    IConfiguration configuration) : IAuthService
{
    public async Task<(bool Success, string? ErrorMessage, AuthResponseDto? Data)> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (await userRepository.ExistsByEmailAsync(dto.Email, cancellationToken).ConfigureAwait(false))
        {
            return (false, "Користувач з таким Email вже існує.", null);
        }

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await userRepository.AddAsync(user, cancellationToken).ConfigureAwait(false);

        var token = GenerateJwtToken(user);
        var response = new AuthResponseDto(token, user.Email, user.Id);

        return (true, null, response);
    }

    public async Task<(bool Success, string? ErrorMessage, AuthResponseDto? Data)> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var user = await userRepository.GetByEmailAsync(dto.Email, cancellationToken).ConfigureAwait(false);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return (false, "Невірний Email або пароль.", null);
        }

        var token = GenerateJwtToken(user);
        var response = new AuthResponseDto(token, user.Email, user.Id);

        return (true, null, response);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("Ключ JWT (Jwt:Key) не знайдено в appsettings.json! Додай його.");
        }

        var key = Encoding.ASCII.GetBytes(jwtKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Email, user.Email)
                    ]),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}