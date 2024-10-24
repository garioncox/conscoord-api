namespace conscoord_api.Data;
public class CustomConfiguration
{
    public required string SMTP_SENDERNAME { get; set; }
    public required string SMTP_USERNAME { get; set; }
    public required string SMTP_PASSWORD { get; set; }
    public required string DB { get; set; }
    public required bool EMAIL_ENABLED { get; set; }
}
