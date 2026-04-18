namespace Wasl.Application.Interfaces
{
    public interface IAuthService
    {
        //Register User
        Task<ResultResponse<RegisterResponse>> RegisterAsync(RegisterDto registerDto);

        //Login User
        Task<ResultResponse<LoginResponse>> LoginAsync(LoginDto loginDto);

        //ConfirmEmail
        Task<ResultResponse<ConfirmEmailResponse>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);

        //RefreshToken
        Task<ResultResponse<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);

        //ForgetPassword
        Task<ResultResponse<string>> ForgetPasswordAsync(ForgotPasswordDto forgotPasswordDto);

        //Reset Password
        public Task<ResultResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);



    }
}
