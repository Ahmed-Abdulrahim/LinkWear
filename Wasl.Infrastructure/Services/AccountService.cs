namespace Wasl.Infrastructure.Services
{
    public class AccountService(IMapper mapper, UserManager<ApplicationUser> userManager,
        IHttpContextAccessor http, IUnitOfWork unitOfWork, ILogger<AccountService> logger) : IAccountService
    {
        //Get User Profile
        public async Task<ResultResponse<UserProfileResponse>> GetProfileAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return ResultResponse<UserProfileResponse>.Failure("User not found.");

            var profile = new UserProfileResponse
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = user.UserType.ToString(),
                BusinessName = user.BusinessName,
                IsDeleted = user.IsDeleted,
                ApprovalStatus = user.ApprovalStatus,
                City = user.City,
                BusinessDescription = user.BusinessDescription,
                ActivityType = user.ActivityType,
            };
            return ResultResponse<UserProfileResponse>.Success(profile);

        }

        //Update User Profile
        public async Task<ResultResponse<UserProfileResponse>> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return ResultResponse<UserProfileResponse>.Failure("User not found.");
            user.PhoneNumber = dto.PhoneNumber;
            user.BusinessName = dto.BusinessName;
            user.BusinessDescription = dto.BusinessDescription;


            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return ResultResponse<UserProfileResponse>.Failure(string.Join(", ", errors));
            }

            logger.LogInformation("Profile updated for user {UserId}", userId);
            var profile = mapper.Map<UserProfileResponse>(user);
            return ResultResponse<UserProfileResponse>.Success(profile);
        }

        //Change Password
        public async Task<ResultResponse<string>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return ResultResponse<string>.Failure("User not found.");
            var result = await userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return ResultResponse<string>.Failure(errors);
            }
            await LogoutAsync(userId);
            logger.LogInformation("Password changed for user {UserId} , Plz Login again", userId);
            return ResultResponse<string>.Success("Password Changed Success");
        }

        //Logout
        public async Task<ResultResponse<string>> LogoutAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return ResultResponse<string>.Failure("User not found.");
            var spec = new RefreshTokenSpecification(rt => rt.UserId == user.Id && rt.IsRevoked == false && rt.ExpiresAt >= DateTime.UtcNow);
            var refreshTokens = await unitOfWork.Repository<RefreshToken>().GetAllSpecTrackedAsync(spec);
            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }
            await unitOfWork.CommitAsync();
            return ResultResponse<string>.Success("Logout Success");
        }
    }
}
