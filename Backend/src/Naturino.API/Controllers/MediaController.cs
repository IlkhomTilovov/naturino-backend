using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naturino.Application.DTOs.Media;
using Naturino.Application.Services;
using Naturino.Domain.Exceptions;

namespace Naturino.API.Controllers;

[ApiController]
[Route("api/v1/media")]
[Authorize]
public class MediaController : ControllerBase
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MediaFileDto>>> GetAll(CancellationToken ct)
    {
        return Ok(await _mediaService.GetAllAsync(ct));
    }

    [HttpPost("upload")]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<ActionResult<MediaFileDto>> Upload([FromForm] IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            throw new Domain.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["file"] = ["Fayl tanlanmagan."]
            });
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new Domain.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["file"] = ["Fayl hajmi 10 MB dan oshmasligi kerak."]
            });
        }

        var userId = GetUserId();

        await using var stream = file.OpenReadStream();
        var result = await _mediaService.UploadAsync(stream, file.FileName, file.ContentType, file.Length, userId, ct);

        return Ok(result);
    }

    [HttpPost("from-url")]
    public async Task<ActionResult<MediaFileDto>> CreateFromUrl([FromBody] CreateMediaFromUrlDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        var result = await _mediaService.CreateFromUrlAsync(dto, userId, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediaService.DeleteAsync(id, ct);
        return NoContent();
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
