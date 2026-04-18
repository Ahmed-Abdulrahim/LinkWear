namespace Wasl.Application.Validation
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(r => r.Email).NotEmpty().WithMessage("Email is Required")
                .EmailAddress().WithMessage("Invalid Email Format");

            RuleFor(r => r.Token).NotEmpty().WithMessage("Token is Required");
            RuleFor(r => r.NewPassword).NotEmpty().WithMessage("New Password is Required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(r => r.ConfirmPassword).NotEmpty().WithMessage("confirm Password is Required")
               .Equal(r => r.NewPassword).WithMessage("Passwords do not match");

        }
    }
}
