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
    }

    private readonly string _storageAccount = "invoices";
    private readonly string _key;
    private readonly BlobContainerClient _filesContainer;

}
