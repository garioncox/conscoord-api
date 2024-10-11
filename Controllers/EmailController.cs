using conscoord_api.Data;
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
    private readonly FeatureFlags _featureFlags;

    public EmailController(ILogger<EmailController> logger, IOptions<SmtpSettings> smtpSettings, IOptions<FeatureFlags> featureFlags)
    {
        _logger = logger;
        _smtpSettings = smtpSettings.Value;
        _featureFlags = featureFlags.Value;
    }

    public IActionResult SendEmail(string email, string subject, string messageBody)
    {
        if (_featureFlags.EMAIL_ENABLED)
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
        else
        {
            return StatusCode(405);
        }
    }

    [HttpPost]
    [Route("send")]
    public IActionResult SendEmail([FromBody] EmailRequest emailRequest)
    {
        if (_featureFlags.EMAIL_ENABLED)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.Username));
            message.To.Add(new MailboxAddress("", emailRequest.Email));
            message.Subject = emailRequest.Subject;
            message.Body = new TextPart("plain") { Text = emailRequest.MessageBody };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(_smtpSettings.Username, _smtpSettings.Password);
                client.Send(message);
                client.Disconnect(true);
            }

            return Ok("Success");
        }
        else
        {
            return StatusCode(405);
        }
    }
}

public class SmtpSettings
{
    public required string SenderName { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}