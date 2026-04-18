namespace Wasl.Infrastructure.Services
{
    public class FcmService(ILogger<FcmService> logger, IUnitOfWork unitOfWork) : IFcmService
    {
        public async Task SendAsync(List<string> tokens, string title, string body)
        {
            if (tokens == null || !tokens.Any())
                return;

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                var response = await FirebaseMessaging
                    .DefaultInstance
                    .SendEachForMulticastAsync(message);

                if (response.FailureCount > 0)
                {
                    for (int i = 0; i < response.Responses.Count; i++)
                    {
                        var result = response.Responses[i];

                        if (!result.IsSuccess)
                        {
                            var exception = result.Exception as FirebaseMessagingException;

                            if (exception != null &&
                                (exception.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                                 exception.MessagingErrorCode == MessagingErrorCode.InvalidArgument))
                            {
                                var invalidToken = tokens[i];

                                var spec = new FcmTokenSpecification(f => f.Token == invalidToken);
                                // Remove invalid token from DB
                                var tokenEntity = await unitOfWork
                                    .Repository<FcmToken>()
                                    .GetByIdSpecAsync(spec);

                                if (tokenEntity != null)
                                {
                                    unitOfWork.Repository<FcmToken>().Delete(tokenEntity);
                                }
                            }

                            logger.LogWarning(
                                "FCM failed for token: {Token}, Error: {Error}",
                                tokens[i],
                                result.Exception?.Message
                            );
                        }
                    }

                    await unitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while sending FCM notification");
            }
        }
    }
}
