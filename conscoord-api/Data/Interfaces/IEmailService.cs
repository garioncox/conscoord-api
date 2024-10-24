using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Data.Interfaces;
public interface IEmailService
{
    public IActionResult SendEmail(string email, string subject, string messageBody);
    public IActionResult SendEmail([FromBody] EmailRequest emailRequest);
}
