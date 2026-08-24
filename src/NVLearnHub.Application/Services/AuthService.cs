using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NVLearnHub.Application.DTOs.Auth;
using NVLearnHub.Application.Interfaces;
using NVLearnHub.Application.Interfaces.Services;
using NVLearnHub.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace NVLearnHub.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork uow, IConfiguration config, IEmailService emailService)
        {
            _uow = uow;
            _config = config;
            _emailService = emailService;
        }

        // ─── Register ────────────────────────────────────────────────────────

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            // 1. Validate verification token
            var emailOtp = await _uow.EmailOtps
                .GetValidVerificationTokenAsync(dto.VerificationToken);

            if (emailOtp == null)
                return new ApiResponse<AuthResponseDto>(false, "Email verification required.", 400);

            // 2. Make sure token email matches registration email
            if (emailOtp.Email != dto.Email)
                return new ApiResponse<AuthResponseDto>(false, "Email mismatch.", 400);

            // 3. Check email not already registered
            if (await _uow.Users.EmailExistsAsync(dto.Email))
                return new ApiResponse<AuthResponseDto>(false, "Email is already registered.", 409);

            // 4. Passwords match
            if (dto.Password != dto.ConfirmPassword)
                return new ApiResponse<AuthResponseDto>(false, "Passwords do not match.", 400);

            // 5. Get student role
            var roles = await _uow.Roles.GetAllAsync();
            var studentRole = roles.FirstOrDefault(r => r.Name == "Student");
            if (studentRole == null)
                return new ApiResponse<AuthResponseDto>(false, "Student role not found.", 404);

            // 6. Create user
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = studentRole.Id,
                IsActive = true,
                YearsOfExperience = dto.YearsOfExperience,
                AreaOfExpertise = dto.AreaOfExpertise,
                Expertise = dto.Expertise ?? (string.IsNullOrWhiteSpace(dto.AreaOfExpertise)
                    ? new List<string>()
                    : new List<string> { dto.AreaOfExpertise })
            };

            await _uow.Users.AddAsync(user);

            // 7. Mark OTP token as used
            emailOtp.IsUsed = true;
            _uow.EmailOtps.Update(emailOtp);

            await _uow.SaveChangesAsync();

            var data = GenerateAuthResponse(user, studentRole.Name);
            return new ApiResponse<AuthResponseDto>(data, "Registration successful.");
        }

        // ─── Login ───────────────────────────────────────────────────────────

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _uow.Users.GetByEmailAsync(dto.Email);
            if (user == null)
                return new ApiResponse<AuthResponseDto>(false, "No account found with that email.");

            if (!user.IsActive)
                return new ApiResponse<AuthResponseDto>(false, "Account is inactive. Please contact support.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new ApiResponse<AuthResponseDto>(false, "Invalid email or password.");

            var data = GenerateAuthResponse(user, user.Role.Name);
            return new ApiResponse<AuthResponseDto>(data, "Login successful.");
        }

        // ─── Forgot Password ─────────────────────────────────────────────────

        public async Task<ApiResponse<ForgotPasswordResponseDto>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _uow.Users.GetByEmailAsync(dto.Email);
            if (user == null)
                return new ApiResponse<ForgotPasswordResponseDto>(false, "No account found with that email.");

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

            var data = new ForgotPasswordResponseDto { Token = token };
            return new ApiResponse<ForgotPasswordResponseDto>(data, "Reset token generated.");
        }

        // ─── Reset Password ──────────────────────────────────────────────────

        public async Task<ApiResponse<ResetPasswordResponseDto>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return new ApiResponse<ResetPasswordResponseDto>(false, "Passwords do not match.");

            var resetToken = await _uow.PasswordResetTokens.GetValidTokenAsync(dto.Token);
            if (resetToken == null)
                return new ApiResponse<ResetPasswordResponseDto>(false, "Invalid or expired reset token.");

            var user = await _uow.Users.GetByIdAsync(resetToken.UserId);
            if (user == null)
                return new ApiResponse<ResetPasswordResponseDto>(false, "User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _uow.Users.Update(user);

            resetToken.IsUsed = true;
            _uow.PasswordResetTokens.Update(resetToken);

            await _uow.SaveChangesAsync();

            var data = new ResetPasswordResponseDto { IsReset = true };
            return new ApiResponse<ResetPasswordResponseDto>(data, "Password reset successful.");
        }

        // ─── Send OTP ─────────────────────────────────────────────────────────────

        public async Task<ApiResponse<SendOtpResponseDto>> SendOtpAsync(SendOtpDto dto)
        {
            if (await _uow.Users.EmailExistsAsync(dto.Email))
                return new ApiResponse<SendOtpResponseDto>(false, "Email is already registered.", 409);

            await _uow.EmailOtps.InvalidatePreviousOtpsAsync(dto.Email);
            await _uow.SaveChangesAsync();

            var masterOtp = _config["Email:MasterOtp"];
            var otpCode = string.IsNullOrWhiteSpace(masterOtp)
                ? new Random().Next(100000, 999999).ToString()
                : masterOtp; // ← use master OTP for demo

            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            var emailOtp = new EmailOtp
            {
                Email = dto.Email,
                OtpCode = otpCode,
                ExpiresAt = expiresAt,
                IsVerified = false,
                IsUsed = false
            };

            await _uow.EmailOtps.AddAsync(emailOtp);
            await _uow.SaveChangesAsync();

            // Only send real email if no master OTP configured
            if (string.IsNullOrWhiteSpace(masterOtp))
                await _emailService.SendOtpEmailAsync(dto.Email, otpCode);

            var data = new SendOtpResponseDto
            {
                Email = dto.Email,
                ExpiresAt = expiresAt
            };

            return new ApiResponse<SendOtpResponseDto>(data, "OTP sent to your email.");
        }

        // ─── Verify OTP ───────────────────────────────────────────────────────────

        public async Task<ApiResponse<VerifyOtpResponseDto>> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var masterOtp = _config["Email:MasterOtp"];
            var isMasterOtp = !string.IsNullOrWhiteSpace(masterOtp)
                              && dto.OtpCode == masterOtp;

            EmailOtp? emailOtp;

            if (isMasterOtp)
            {
                // Master OTP path — find any pending OTP record for this email
                emailOtp = await _uow.EmailOtps.GetPendingOtpByEmailAsync(dto.Email);
                if (emailOtp == null)
                    return new ApiResponse<VerifyOtpResponseDto>(false, "No OTP request found for this email.", 400);
            }
            else
            {
                // Normal path — validate OTP code from DB
                emailOtp = await _uow.EmailOtps.GetValidOtpAsync(dto.Email, dto.OtpCode);
                if (emailOtp == null)
                    return new ApiResponse<VerifyOtpResponseDto>(false, "Invalid or expired OTP.", 400);
            }

            var verificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

            emailOtp.IsVerified = true;
            emailOtp.VerificationToken = verificationToken;
            emailOtp.ExpiresAt = DateTime.UtcNow.AddMinutes(30);
            _uow.EmailOtps.Update(emailOtp);
            await _uow.SaveChangesAsync();

            var data = new VerifyOtpResponseDto
            {
                VerificationToken = verificationToken,
                Email = dto.Email,
                ExpiresAt = emailOtp.ExpiresAt
            };

            return new ApiResponse<VerifyOtpResponseDto>(data, "Email verified successfully.");
        }

        // ─── JWT Helper ──────────────────────────────────────────────────────

        private AuthResponseDto GenerateAuthResponse(User user, string roleName)
        {
            var secret = _config["Jwt:Secret"]!;
            var issuer = _config["Jwt:Issuer"]!;
            var audience = _config["Jwt:Audience"]!;
            var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes); // 4320 = 3days

            var claims = new[]
            {
                new Claim(JwtClaims.Sub,   user.Id.ToString()),
                new Claim(JwtClaims.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("fullName",      user.FullName),
                new Claim("firstName",     user.FirstName),
                new Claim("lastName",      user.LastName)
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
                FullName = user.FullName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = roleName,
                YearsOfExperience = user.YearsOfExperience,
                AreaOfExpertise = user.AreaOfExpertise,
                Expertise = user.Expertise,
                ExpiresAt = expiry
            };
        }
    }
}