namespace Wasl.Application.Interfaces
{
    public interface IAccountService
    {
        //Get Profile
        Task<ResultResponse<UserProfileResponse>> GetProfileAsync(string userId);

        //UpdateProfile
        Task<ResultResponse<UserProfileResponse>> UpdateProfileAsync(string userId, UpdateProfileDto dto);

        //Change Password
        Task<ResultResponse<string>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);

        //Logout
        Task<ResultResponse<string>> LogoutAsync(string userId);
    }
}
