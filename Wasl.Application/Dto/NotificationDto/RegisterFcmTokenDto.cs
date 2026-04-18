namespace Wasl.Application.Dto.NotificationDto
{
    public class RegisterFcmTokenDto
    {
        [Required]
        public string Token { get; set; }
    }
}
