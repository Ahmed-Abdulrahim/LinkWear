namespace Wasl.Application.Validation
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(c => c.NewPassword).NotEmpty().WithMessage("Password is required")
               .MinimumLength(8).MaximumLength(100).WithMessage("Password must be between 8 and 100 characters")
               .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
               .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number and one special character");

            RuleFor(c => c.ConfirmNewPassword).NotEmpty().WithMessage("Confirm Password is required")
                .Equal(r => r.NewPassword).WithMessage("New Password and confirmation password do not match");
        }
    }
}
