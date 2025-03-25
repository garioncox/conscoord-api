using System.Reflection.Metadata;

namespace conscoord_api.Data.DTOs;

public class AzureResponseDTO
{
    public AzureResponseDTO()
    {
        Blob = new AzureInvoiceDTO();
    }
    public string? Status { get; set; }
    public bool Error { get; set; }
    public AzureInvoiceDTO Blob { get; set; }
}
