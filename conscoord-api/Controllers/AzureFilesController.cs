using conscoord_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AzureFilesController : Controller
{
    private readonly AzureFileService _fileService;
    public AzureFilesController(AzureFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllFiles()
    {
        var result = await _fileService.GetAll();
        return Ok(result);
    }

    [HttpPost()]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var result = await _fileService.uploadAsync(file);
        return Ok();
    }

    [HttpGet("Download/{fileName}")]
    public async Task<IActionResult> Download(string fileName)
    {
        var result = await _fileService.DownloadAsync(fileName);
        if (result is not null && result.Content is not null && result.ContentType is not null)
        {
            return File(result.Content, result.ContentType, result.Name);
        }
        return NotFound();
    }
}
