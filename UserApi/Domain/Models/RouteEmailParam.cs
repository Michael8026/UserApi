namespace api.Models;

public partial class RouteEmailParam
{
    public string Id { get; set; }

    public string MailSender { get; set; }

    public string Password { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public bool UseSSL { get; set; }

    public string DefaultWebsite { get; set; }


}

