using Azure.Storage.Blobs;
using conscoord_api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace conscoord_api.Services;

public class FileService
{
    private readonly CustomConfiguration _configurations;

    public FileService(IOptions<CustomConfiguration> customConfiguration)
    {
        _configurations = customConfiguration.Value;
        _key = _configurations.AZURE_KEY;

        //https://www.youtube.com/watch?v=DzQ7CNnb9yM @3:40
        var credential = new StorageShareKeyCredential(_storageAccount, _key);
        var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        var blobServiceClinet = new BlobServiceClient(new Uri(blobUri), credential);
        _filesContainer = blobServiceClinet.GetBlobContainerClient("files");
    }

    private readonly string _storageAccount = "invoices";
    private readonly string _key;
    private readonly BlobContainerClient _filesContainer;

}
