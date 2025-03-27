namespace conscoord_api.Data.DTOs;

public class AzureInvoiceDTO
{
    public Guid Id { get; set; }
    public string? URI { get; set; }
    public string? Name { get; set; }
    public string? ContentType { get; set; }
    public Stream? Content { get; set; }
}
