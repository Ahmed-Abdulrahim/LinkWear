namespace Wasl.Application.Response
{
    public class RegisterResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNUmber { get; set; }
        public bool RequiresEmailConfirmation { get; set; }
    }
}
