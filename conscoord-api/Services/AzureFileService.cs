using Azure.Storage;
using Azure.Storage.Blobs;
using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace conscoord_api.Services;

//taken from
//https://www.youtube.com/watch?v=DzQ7CNnb9yM 

public class AzureFileService
{
    private readonly CustomConfiguration _configurations;
    private readonly string _storageAccount = "practicuminvoice";
    private readonly string _key;
    private readonly BlobContainerClient _filesContainer;

    public AzureFileService(IOptions<CustomConfiguration> customConfiguration)
    {
        _configurations = customConfiguration.Value;
        _key = _configurations.AZURE_KEY;

        var credential = new StorageSharedKeyCredential(_storageAccount, _key);
        var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        var blobServiceClinet = new BlobServiceClient(new Uri(blobUri), credential);
        _filesContainer = blobServiceClinet.GetBlobContainerClient("invoices");
    }


    public async Task<List<AzureInvoiceDTO>> GetAll()
    {
        var files = new List<AzureInvoiceDTO>();
        await foreach (var file in _filesContainer.GetBlobsAsync())
        {
            var uri = _filesContainer.Uri.ToString();
            var fullUri = $"{uri}/{file.Name}";

            files.Add(new AzureInvoiceDTO
            {
                URI= fullUri,
                Name= file.Name,
                ContentType = file.Properties.ContentType
            });
        }
        return files;
    }

    public async Task<AzureResponseDTO> uploadAsync(IFormFile file)
    {
        AzureResponseDTO response = new();
        var client = _filesContainer.GetBlobClient(file.FileName);

        try
        {
            await using (var data = file.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {file.FileName} uploaded successfully";
            response.Error = false;
            response.Blob.URI = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;
        }
        catch(Exception e)
        {
            response.Status = $"File {file.FileName} did not upload. Error {e.Message}";
            response.Error = true;
        }

        return response;
    }

    public async Task<AzureInvoiceDTO?> DownloadAsync(string fileName)
    {
        var file = _filesContainer.GetBlobClient(fileName);

        if (await file.ExistsAsync())
        {
            var data = await file.OpenReadAsync();
            var blobContent = data;

            var content = await file.DownloadContentAsync();

            var contentType = content.Value.Details.ContentType;

            return new AzureInvoiceDTO { Content = blobContent, Name = fileName, ContentType = contentType };
        }
        return null;
    }
}
