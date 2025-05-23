using api.Interfaces;
using MailKit.Security;
using MimeKit;
using static api.Helpers.CommunicationData;

namespace api.Service;
public partial class EmailService : IEmailService
{
    private readonly IRouteEmailParamRepository _routeEmailParamRepository;
    public EmailService(IRouteEmailParamRepository routeEmailParamRepository)
    {
        _routeEmailParamRepository = routeEmailParamRepository;
    }


    public Task<bool> SendConfirmationEmail(EmailUserDTO user, string token)
    {
        string confirmationLink = $"http://localhost:5076/confirm-email?token={token}";

        var emailBody = $@"
        <html>
            <body>
                <h3>Welcome to User Api Portal</h3>
                <p>Hi {user.FirstName},</p>
                <p>Please click the button below to confirm your email address:</p>
                <p>
                    <a href='{confirmationLink}' style='padding:10px 20px; background-color:#007BFF; color:white; text-decoration:none; border-radius:5px;'>Confirm Email</a>
                </p>
                <p>If you did not register, please ignore this email.</p>
            </body>
        </html>";

        var emailData = new EmailData
        {
            FullName = $"{user.FirstName} {user.LastName}",
            To = user.Email,
            Subject = "Confirm Your Email - User Api Portal",
            Body = emailBody
        };

        var success = SendMail(emailData);
        return Task.FromResult(success);
    }



    public bool SendMail(EmailData model)
    {
        var Rmps = _routeEmailParamRepository.GetRouteEmailParams();

        var mailSender = Rmps.MailSender;
        var password = Rmps.Password;
        var host = Rmps.Host;
        var ssl = SecureSocketOptions.Auto;
        var port = Rmps.Port;


        try
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("UserApi", mailSender));
            message.To.Add(new MailboxAddress(model.FullName, model.To));
            message.Subject = model.Subject;

            message.Body = new TextPart("html")
            {
                Text = model.Body
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {

                client.Connect(host, port, ssl);

                client.Authenticate(mailSender, password);

                client.Send(message);
                client.Disconnect(true);


            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }












}