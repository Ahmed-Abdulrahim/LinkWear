namespace Wasl.Application.Validation
{
    public class RegisterFcmTokenValidator : AbstractValidator<RegisterFcmTokenDto>
    {
        public RegisterFcmTokenValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("FCM token is required.")
                .MaximumLength(512).WithMessage("FCM token cannot exceed 512 characters.");
        }
    }
}
