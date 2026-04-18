namespace Wasl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(IAuthService authService) : ControllerBase
    {
        //Register
        [HttpPost("register")]
        //[Consumes("multipart/form-data")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            var result = await authService.RegisterAsync(registerDto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        //ConfirmEmail
        [HttpGet("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDto dto)
        {
            var result = await authService.ConfirmEmailAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        //Login
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await authService.LoginAsync(loginDto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //GetNewAccessToken
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RefreshTokenResponse>> GetNewAccessToken(RefreshTokenDto tokenRequest)
        {
            var result = await authService.RefreshTokenAsync(tokenRequest);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //ForgotPassword
        [HttpPost("Forget-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await authService.ForgetPasswordAsync(forgotPasswordDto);
            if (!user.Succeeded)
            {
                return BadRequest(user);
            }
            return Ok(user);
        }

        //Reset Password
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await authService.ResetPasswordAsync(resetPasswordDto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }



    }
}
