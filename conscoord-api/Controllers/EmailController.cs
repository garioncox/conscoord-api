using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase, IEmailService
{
    private readonly ILogger<EmailController> _logger;
    private readonly CustomConfiguration _configurations;

    public EmailController(ILogger<EmailController> logger, IOptions<CustomConfiguration> customConfiguration)
    {
        _logger = logger;
        _configurations = customConfiguration.Value;
    }

    [HttpPost]
    public IActionResult SendEmail(string email, string subject, string messageBody)
    {
        if (_configurations.EMAIL_ENABLED)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configurations.SMTP_SENDERNAME, _configurations.SMTP_USERNAME));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = messageBody };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(_configurations.SMTP_USERNAME, _configurations.SMTP_PASSWORD);
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
        if (_configurations.EMAIL_ENABLED)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configurations.SMTP_SENDERNAME, _configurations.SMTP_USERNAME));
            message.To.Add(new MailboxAddress("", emailRequest.Email));
            message.Subject = emailRequest.Subject;
            message.Body = new TextPart("plain") { Text = emailRequest.MessageBody };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(_configurations.SMTP_USERNAME, _configurations.SMTP_PASSWORD);
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