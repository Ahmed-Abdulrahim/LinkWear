namespace Wasl.Application.Interfaces
{
    public interface IFcmService
    {
        Task SendAsync(List<string> tokens, string title, string body);
    }
}
