using static api.Helpers.CommunicationData;

namespace api.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendConfirmationEmail(EmailUserDTO user, string token);
        bool SendMail(EmailData model);
    }
}