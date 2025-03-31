using conscoord_api.Data.DTOs;

namespace conscoord_api.Data.Interfaces;

public interface IAzureFileService
{
    Task<List<AzureInvoiceDTO>> GetAll();
    Task<AzureResponseDTO> uploadAsync(IFormFile file);
    Task<AzureInvoiceDTO?> DownloadAsync(string fileName);
}
