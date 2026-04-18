namespace Wasl.Application.Dto
{
    public class RegisterDto
    {

        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
        public string? BusinessName { get; set; }

        public string? PhoneNumber { get; set; }
        public ActivityType? ActivityType { get; set; }
        public string? City { get; set; }
        public string? BusinessDescription { get; set; }

    }
}
