using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public interface IDocumentService
{
    Task<DocumentDto> CreateDocumentAsync(DocumentUploadRequest request);
    Task<List<DocumentDto>> GetDocumentsForUserAsync(int userId, DocumentQueryParameters query);
    Task<List<DocumentDto>> GetSharedDocumentsAsync(int userId, DocumentQueryParameters query);
    Task<List<DocumentDto>> GetDocumentsForProjectAsync(int projectId, int userId, DocumentQueryParameters query);
    Task<List<DocumentDto>> GetRecentDocumentsAsync(int userId, int count = 5);
    Task<Document?> GetDocumentByIdAsync(int documentId);
    Task<bool> UserHasAccessAsync(int documentId, int userId);
}

public class DocumentService : IDocumentService
{
    private const long MaxUploadSizeBytes = 25 * 1024 * 1024;
    private static readonly string[] SupportedMimeTypes = new[]
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain",
        "image/jpeg",
        "image/png"
    };

    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly IVirusScanQueueService _virusScanQueue;

    public DocumentService(
        ApplicationDbContext context,
        IFileStorageService fileStorage,
        IVirusScanQueueService virusScanQueue)
    {
        _context = context;
        _fileStorage = fileStorage;
        _virusScanQueue = virusScanQueue;
    }

    public async Task<DocumentDto> CreateDocumentAsync(DocumentUploadRequest request)
    {
        if (request.FileStream == null)
        {
            throw new ArgumentNullException(nameof(request.FileStream));
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Document title is required.", nameof(request.Title));
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            throw new ArgumentException("Document category is required.", nameof(request.Category));
        }

        if (request.FileStream.Length > MaxUploadSizeBytes)
        {
            throw new InvalidOperationException("File size exceeds the maximum allowed upload limit of 25 MB.");
        }

        if (!SupportedMimeTypes.Contains(request.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Unsupported file type.");
        }

        var storagePath = await _fileStorage.UploadAsync(
            request.FileStream,
            request.UploadedByUserId.ToString(),
            request.ProjectId?.ToString() ?? "personal",
            request.OriginalFileName,
            request.ContentType);

        var document = new Document
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Category = request.Category.Trim(),
            ProjectId = request.ProjectId,
            UploaderId = request.UploadedByUserId,
            UploadDate = DateTime.UtcNow,
            OriginalFileName = request.OriginalFileName,
            FilePath = storagePath,
            FileType = request.ContentType,
            FileSize = request.FileStream.Length,
            Tags = request.Tags is { Count: > 0 }
                ? string.Join(",", request.Tags.Where(tag => !string.IsNullOrWhiteSpace(tag)).Select(tag => tag.Trim()))
                : null,
            ScanStatus = DocumentScanStatus.Pending,
            UpdatedDate = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        try
        {
            await _virusScanQueue.EnqueueVirusScanAsync(document.DocumentId, document.FilePath, document.FileType, document.OriginalFileName, document.UploaderId);
        }
        catch
        {
            // Virus scan enqueue failures should not block the upload flow for training/demo scenarios.
        }

        return MapToDto(document);
    }

    public async Task<List<DocumentDto>> GetDocumentsForUserAsync(int userId, DocumentQueryParameters query)
    {
        IQueryable<Document> documents = _context.Documents
            .Where(d => d.UploaderId == userId)
            .Include(d => d.Project)
            .Include(d => d.Uploader)
            .OrderByDescending(d => d.UploadDate);

        documents = ApplyQueryFilters(documents, query);

        var documentPage = await documents
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return documentPage.Select(MapToDto).ToList();
    }

    public async Task<List<DocumentDto>> GetSharedDocumentsAsync(int userId, DocumentQueryParameters query)
    {
        IQueryable<Document> documents = _context.Documents
            .Include(d => d.Project)
            .Include(d => d.Uploader)
            .Where(d => d.Shares.Any(s => s.RecipientUserId == userId) ||
                        (d.ProjectId != null && d.Project.ProjectMembers.Any(pm => pm.UserId == userId)))
            .OrderByDescending(d => d.UploadDate);

        documents = ApplyQueryFilters(documents, query);

        var documentPage = await documents
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return documentPage.Select(d => MapToDto(d, d.Shares.Any(s => s.RecipientUserId == userId))).ToList();
    }

    public async Task<List<DocumentDto>> GetDocumentsForProjectAsync(int projectId, int userId, DocumentQueryParameters query)
    {
        var hasAccess = await _context.Projects
            .AnyAsync(p => p.ProjectId == projectId &&
                           (p.ProjectManagerId == userId || p.ProjectMembers.Any(pm => pm.UserId == userId)));

        if (!hasAccess)
        {
            return new List<DocumentDto>();
        }

        IQueryable<Document> documents = _context.Documents
            .Where(d => d.ProjectId == projectId)
            .Include(d => d.Project)
            .Include(d => d.Uploader)
            .OrderByDescending(d => d.UploadDate);

        documents = ApplyQueryFilters(documents, query);

        var documentPage = await documents
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return documentPage.Select(MapToDto).ToList();
    }

    private IQueryable<Document> ApplyQueryFilters(IQueryable<Document> documents, DocumentQueryParameters query)
    {
        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            var searchText = query.SearchText.Trim().ToLower();
            documents = documents.Where(d =>
                d.Title.ToLower().Contains(searchText) ||
                (d.Description != null && d.Description.ToLower().Contains(searchText)) ||
                (d.Tags != null && d.Tags.ToLower().Contains(searchText)) ||
                d.Uploader.DisplayName.ToLower().Contains(searchText) ||
                (d.Project != null && d.Project.Name.ToLower().Contains(searchText)));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            documents = documents.Where(d => d.Category == query.Category);
        }

        if (query.ProjectId.HasValue)
        {
            documents = documents.Where(d => d.ProjectId == query.ProjectId.Value);
        }

        if (query.CreatedFrom.HasValue)
        {
            documents = documents.Where(d => d.UploadDate >= query.CreatedFrom.Value);
        }

        if (query.CreatedTo.HasValue)
        {
            documents = documents.Where(d => d.UploadDate <= query.CreatedTo.Value);
        }

        return documents;
    }

    public async Task<List<DocumentDto>> GetRecentDocumentsAsync(int userId, int count = 5)
    {
        var documents = await _context.Documents
            .Where(d => d.UploaderId == userId)
            .Include(d => d.Project)
            .Include(d => d.Uploader)
            .OrderByDescending(d => d.UploadDate)
            .Take(count)
            .ToListAsync();

        return documents.Select(MapToDto).ToList();
    }

    public async Task<Document?> GetDocumentByIdAsync(int documentId)
    {
        return await _context.Documents
            .Include(d => d.Project)
            .Include(d => d.Uploader)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId);
    }

    public async Task<bool> UserHasAccessAsync(int documentId, int userId)
    {
        var authorized = await _context.Documents
            .AnyAsync(d => d.DocumentId == documentId && (d.UploaderId == userId || d.Shares.Any(s => s.RecipientUserId == userId)));

        if (authorized)
        {
            return true;
        }

        var projectAccess = await _context.Documents
            .AnyAsync(d => d.DocumentId == documentId && d.ProjectId != null && d.Project.ProjectMembers.Any(pm => pm.UserId == userId));

        return projectAccess;
    }

    private static DocumentDto MapToDto(Document document)
    {
        return new DocumentDto
        {
            DocumentId = document.DocumentId,
            Title = document.Title,
            Description = document.Description,
            Category = document.Category,
            ProjectId = document.ProjectId,
            ProjectName = document.Project?.Name,
            UploadedByUserId = document.UploaderId,
            UploadedByName = document.Uploader.DisplayName,
            UploadDate = document.UploadDate,
            FileSize = document.FileSize,
            FileType = document.FileType,
            IsPreviewable = document.IsPreviewable,
            Tags = string.IsNullOrWhiteSpace(document.Tags)
                ? new List<string>()
                : document.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(tag => tag.Trim()).ToList(),
            ScanStatus = document.ScanStatus.ToString(),
            OriginalFileName = document.OriginalFileName,
            IsSharedWithMe = false
        };
    }

    private static DocumentDto MapToDto(Document document, bool isSharedWithMe)
    {
        var dto = MapToDto(document);
        dto.IsSharedWithMe = isSharedWithMe;
        return dto;
    }
}
