namespace Wasl.Application.Validation
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(f => f.Email).NotEmpty().WithMessage("Email is Required")
                .EmailAddress().WithMessage("Invalid Email Format");
        }
    }
}
