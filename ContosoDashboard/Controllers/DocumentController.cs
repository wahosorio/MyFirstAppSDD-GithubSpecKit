using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContosoDashboard.Services;

namespace ContosoDashboard.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly IFileStorageService _fileStorage;

    public DocumentController(IDocumentService documentService, IFileStorageService fileStorage)
    {
        _documentService = documentService;
        _fileStorage = fileStorage;
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        if (!await _documentService.UserHasAccessAsync(id, userId.Value))
        {
            return Forbid();
        }

        var document = await _documentService.GetDocumentByIdAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        var stream = await _fileStorage.OpenReadAsync(document.FilePath);
        return File(stream, document.FileType, document.OriginalFileName);
    }

    [HttpGet("{id}/preview")]
    public async Task<IActionResult> Preview(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        if (!await _documentService.UserHasAccessAsync(id, userId.Value))
        {
            return Forbid();
        }

        var document = await _documentService.GetDocumentByIdAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        if (!document.IsPreviewable)
        {
            return BadRequest("This file type cannot be previewed in the browser.");
        }

        var stream = await _fileStorage.OpenReadAsync(document.FilePath);
        return File(stream, document.FileType);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return null;
        }

        if (int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }
}
