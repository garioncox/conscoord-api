using System.Reflection.Metadata;

namespace conscoord_api.Data.DTOs;

public class AzureResponseDTO
{
    public AzureResponseDTO()
    {
        Blob = new InvoiceToAzureDTO();
    }
    public string? Status { get; set; }
    public bool Error { get; set; }
    public InvoiceToAzureDTO Blob { get; set; }
}
