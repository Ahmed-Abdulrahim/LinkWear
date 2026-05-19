namespace Wasl.Infrastructure.Services
{
    public class AuthService(IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager,
       UserManager<ApplicationUser> userManager, IOptions<EmailSettings> _emailSettings,
       IEmailService emailService, ITokenService tokenService, ILogger<AuthService> logger, IAccountService accountService
        ) : IAuthService
    {
        private readonly EmailSettings emailSettings = _emailSettings.Value;

        public async Task<ResultResponse<RegisterResponse>> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
                return ResultResponse<RegisterResponse>.Failure(new[] { "Email is Already Exist" });

            if (registerDto.Password != registerDto.ConfirmPassword)
                return ResultResponse<RegisterResponse>.Failure(new[] { "Passwords do not match" });

            var userType = StaticMethod.MapRoleToUserType(registerDto.Role);



            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = GenerateUsername(),
                BusinessName = registerDto.BusinessName,
                PhoneNumber = registerDto.PhoneNumber,
                ActivityType = registerDto.ActivityType,
                City = registerDto.City,
                BusinessDescription = registerDto.BusinessDescription,
                IsDeleted = false,
                ApprovalStatus = ApprovalStatus.Approved,
                UserType = userType,
                CreatedAt = DateTime.UtcNow,
            };

            IdentityResult createUser;
            do
            {
                user.UserName = GenerateUsername();
                createUser = await userManager.CreateAsync(user, registerDto.Password);
            }
            while (!createUser.Succeeded && createUser.Errors.Any(e => e.Code == "DuplicateUserName"));

            if (!createUser.Succeeded)
            {
                var errors = createUser.Errors.Select(e => e.Description);
                return ResultResponse<RegisterResponse>.Failure(errors);
            }

            var addRole = await userManager.AddToRoleAsync(user, registerDto.Role);
            if (!addRole.Succeeded)
            {
                var errors = addRole.Errors.Select(e => e.Description);
                return ResultResponse<RegisterResponse>.Failure(errors);
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{emailSettings.BaseUrl}/api/authentication/confirm-email?email={UrlEncoder.Default.Encode(user.Email!)}&token={encodedToken}";

            try
            {
                await emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            }

            logger.LogInformation("User {Email} registered successfully", user.Email);

            var registerResponse = new RegisterResponse
            {
                Email = user.Email!,
                Id = user.Id,
                RequiresEmailConfirmation = true,
                UserName = user.UserName!,
                PhoneNUmber = user.PhoneNumber!,
            };

            return ResultResponse<RegisterResponse>.Success(registerResponse, "Registration successful. Please check your email to confirm your account.");
        }

        //ConfirmEmail
        public async Task<ResultResponse<ConfirmEmailResponse>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            var user = await userManager.FindByEmailAsync(confirmEmailDto.Email);
            if (user == null)
            {
                return ResultResponse<ConfirmEmailResponse>.Failure("User not found.");
            }

            if (user.EmailConfirmed)
            {
                return ResultResponse<ConfirmEmailResponse>.Success("Already Confirmed");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmEmailDto.Token));
            var result = await userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return ResultResponse<ConfirmEmailResponse>.Failure(string.Join(", ", errors));
            }

            logger.LogInformation("Email confirmed for user {Email}", user.Email);
            return ResultResponse<ConfirmEmailResponse>.Success();
        }

        public async Task<ResultResponse<LoginResponse>> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user is null || user!.IsDeleted == true) return ResultResponse<LoginResponse>.Failure("Invalid email or password.");

            if (!user.EmailConfirmed) return ResultResponse<LoginResponse>.Failure("Email is not confirmed.");

            // Check approval status for non-Admin users
            if (user.UserType != UserType.Admin && user.ApprovalStatus != ApprovalStatus.Approved)
                return ResultResponse<LoginResponse>.Failure("Your account is pending approval. Please wait for admin approval.");

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                return ResultResponse<LoginResponse>.Failure("Invalid email or password.");
            }

            return await GenerateAuthTokensAsync(user);
        }

        //Refresh Token
        public async Task<ResultResponse<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(refreshTokenDto.Token);
            if (principal is null) return ResultResponse<RefreshTokenResponse>.Failure("Invalid access token.");

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return ResultResponse<RefreshTokenResponse>.Failure("Invalid access token.");
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return ResultResponse<RefreshTokenResponse>.Failure("User not found.");

            var specRefreshToken = new RefreshTokenSpecification(rt => rt.Token == refreshTokenDto.RefreshToken && rt.UserId == user.Id && rt.ExpiresAt >= DateTime.UtcNow && rt.IsRevoked == false);
            var getRefreshToken = await unitOfWork.Repository<RefreshToken>().GetByIdSpecTrackedAsync(specRefreshToken);
            if (getRefreshToken is null) return ResultResponse<RefreshTokenResponse>.Failure("Invalid refresh token.");
            var roles = await userManager.GetRolesAsync(user);

            var newAccessToken = await tokenService.GenerateAccesToken(user, roles);
            var newRefeshToken = tokenService.GenerateRefreshtoken();
            var refreshTokenrow = new RefreshToken
            {
                Token = newRefeshToken,
                ExpiresAt = tokenService.GetRefreshTokenExpiration(),
                UserId = user.Id,
            };
            getRefreshToken.IsRevoked = true;
            getRefreshToken.RevokedAt = DateTime.UtcNow;
            unitOfWork.Repository<RefreshToken>().Update(getRefreshToken);
            await unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenrow);
            await unitOfWork.CommitAsync();
            var refreshTokenResponse = new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefeshToken,
                AccessTokenExpiration = tokenService.GetAccessTokenExpiration(),
                RefreshTokenExpiration = tokenService.GetRefreshTokenExpiration(),

            };
            return ResultResponse<RefreshTokenResponse>.Success(refreshTokenResponse);
        }

        //Forget Password
        public async Task<ResultResponse<string>> ForgetPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user is null || !user.EmailConfirmed)
            {
                return ResultResponse<string>.Success();
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var resetLink = $"{emailSettings.BaseUrl}/api/Authentication/reset-password?email={UrlEncoder.Default.Encode(user.Email!)}&token={encodedToken}";

            try
            {
                await emailService.SendPasswordResetAsync(user.Email!, resetLink);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
            }
            return ResultResponse<string>.Success();
        }

        //ResetPassword
        public async Task<ResultResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return ResultResponse<string>.Failure("Invalid request.");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordDto.Token));
            var result = await userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return ResultResponse<string>.Failure(string.Join(", ", errors));
            }

            await accountService.LogoutAsync(user.Id.ToString());

            logger.LogInformation("Password reset successfully for user {Email}", user.Email);
            return ResultResponse<string>.Success();
        }


        //Private Methods
        private async Task<ResultResponse<LoginResponse>> GenerateAuthTokensAsync(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var accessToken = await tokenService.GenerateAccesToken(user, roles);
            var refreshToken = tokenService.GenerateRefreshtoken();


            var refreshTokenrow = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = tokenService.GetRefreshTokenExpiration(),
                UserId = user.Id,
            };
            await unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenrow);
            await unitOfWork.CommitAsync();

            await userManager.UpdateAsync(user);

            var loginResponse = new LoginResponse
            {
                AccessToken = accessToken,
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                RefreshToken = refreshToken,
                AccessTokenExpiration = tokenService.GetAccessTokenExpiration(),
                RefreshTokenExpiration = tokenService.GetRefreshTokenExpiration(),
                Roles = roles.ToList()

            };
            return ResultResponse<LoginResponse>.Success(loginResponse, "Login Successfully");
        }

        public static string GenerateUsername()
        {
            const string letters = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";

            var random = new Random();

            // 4 random letters
            string letterPart = new string(Enumerable.Range(0, 4)
                .Select(_ => letters[random.Next(letters.Length)])
                .ToArray());

            // 3 random digits
            string digitPart = new string(Enumerable.Range(0, 3)
                .Select(_ => digits[random.Next(digits.Length)])
                .ToArray());

            return letterPart + digitPart; // e.g. "xkqm482"
        }
    }
}
