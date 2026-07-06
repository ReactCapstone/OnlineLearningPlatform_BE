using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NVLearnHub.Application.DTOs.Auth;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Services;
using NVLearnHub.Domain.Entities.Identity;

// Alias to resolve ambiguity between
// Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames
// System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames
using JwtClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace NVLearnHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;

    public AuthService(IUnitOfWork uow, IConfiguration config)
    {
        _uow = uow;
        _config = config;
    }

    // ─── Register ────────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // 1. Check if email already exists
        if (await _uow.Users.EmailExistsAsync(dto.Email))
            throw new Exception("Email is already registered.");

        // 2. Validate passwords match
        if (dto.Password != dto.ConfirmPassword)
            throw new Exception("Passwords do not match.");

        // 3. Get Student role (RoleId = 2 by convention, or fetch by name)
        var roles = await _uow.Roles.GetAllAsync();
        var studentRole = roles.FirstOrDefault(r => r.Name == "Student")
                          ?? throw new Exception("Student role not found.");

        // 4. Create user
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId = studentRole.Id,
            IsActive = true
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        // 5. Return JWT
        return GenerateAuthResponse(user, studentRole.Name);
    }

    // ─── Login ───────────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // 1. Find user by email
        var user = await _uow.Users.GetByEmailAsync(dto.Email)
                   ?? throw new Exception("Invalid email or password.");

        // 2. Check account is active
        if (!user.IsActive)
            throw new Exception("Account is inactive. Please contact support.");

        // 3. Verify password
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid email or password.");

        // 4. Return JWT
        return GenerateAuthResponse(user, user.Role.Name);
    }

    // ─── Forgot Password ─────────────────────────────────────────────────────

    public async Task<string> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _uow.Users.GetByEmailAsync(dto.Email)
                   ?? throw new Exception("No account found with that email.");

        // Generate a secure random token
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IsUsed = false
        };

        await _uow.PasswordResetTokens.AddAsync(resetToken);
        await _uow.SaveChangesAsync();

        // In production: send this token via email link
        // e.g. https://yourapp.com/reset-password?token={token}
        // For now, return the token directly so you can test via Swagger
        return token;
    }

    // ─── Reset Password ──────────────────────────────────────────────────────

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            throw new Exception("Passwords do not match.");

        // Find valid, unused, non-expired token
        var resetToken = await _uow.PasswordResetTokens
                             .GetValidTokenAsync(dto.Token)
                         ?? throw new Exception("Invalid or expired token.");

        // Find the user
        var user = await _uow.Users.GetByIdAsync(resetToken.UserId)
                   ?? throw new Exception("User not found.");

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        _uow.Users.Update(user);

        // Mark token as used
        resetToken.IsUsed = true;
        _uow.PasswordResetTokens.Update(resetToken);

        await _uow.SaveChangesAsync();
    }

    // ─── JWT Helper ──────────────────────────────────────────────────────────

    private AuthResponseDto GenerateAuthResponse(User user, string roleName)
    {
        var secret = _config["Jwt:Secret"]!;
        var issuer = _config["Jwt:Issuer"]!;
        var audience = _config["Jwt:Audience"]!;
        var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, roleName),
            new Claim("fullName", user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Email = user.Email,
            FullName = user.FullName,
            Role = roleName,
            ExpiresAt = expiry
        };
    }
}