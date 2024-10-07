using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly ILogger<EmailController> _logger;
    private readonly SmtpSettings _smtpSettings;

    public EmailController(ILogger<EmailController> logger, IOptions<SmtpSettings> smtpSettings)
    {
        _logger = logger;
        _smtpSettings = smtpSettings.Value;
    }

    [HttpPost]
    [Route("send")]
    public IActionResult SendEmail(string email, string subject, string messageBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.Username));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = messageBody };

        using (var client = new SmtpClient())
        {
            client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            client.Authenticate(_smtpSettings.Username, _smtpSettings.Password);
            client.Send(message);
            client.Disconnect(true);
        }

        return Ok("Success");
    }
}

public class SmtpSettings
{
    public string SenderName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}