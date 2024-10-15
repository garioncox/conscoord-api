namespace conscoord_api.Data;
public class CustomConfiguration
{
    public required string DB { get; set; }
    public required bool EMAIL_ENABLED { get; set; } = false;
}