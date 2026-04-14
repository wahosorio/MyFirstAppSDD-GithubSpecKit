# Contracts: Document Upload and Management

## Service Contracts

### IFileStorageService

Interface for filesystem storage operations.

- `Task<string> UploadAsync(Stream fileStream, string fileName, string userId, string projectId, string contentType)`
  - Uploads a file stream to permanent storage.
  - Returns a secure storage key or file path.
- `Task<Stream> DownloadAsync(string storagePath)`
  - Retrieves the stored file stream for download or preview.
- `Task DeleteAsync(string storagePath)`
  - Removes the stored file from disk.
- `Task<bool> ExistsAsync(string storagePath)`
  - Verifies whether the stored file exists.

### IDocumentService

Interface for document metadata and access management.

- `Task<DocumentDto> CreateDocumentAsync(DocumentUploadRequest request)`
- `Task<DocumentDto> UpdateDocumentMetadataAsync(int documentId, DocumentMetadataUpdate request)`
- `Task<DocumentDto> ReplaceDocumentFileAsync(int documentId, Stream fileStream, string fileName, string contentType)`
- `Task DeleteDocumentAsync(int documentId, int userId)`
- `Task<IEnumerable<DocumentDto>> GetDocumentsForUserAsync(int userId, DocumentQueryParameters query)`
- `Task<IEnumerable<DocumentDto>> GetDocumentsForProjectAsync(int projectId, int userId, DocumentQueryParameters query)`
- `Task<IEnumerable<DocumentDto>> GetSharedDocumentsAsync(int userId, DocumentQueryParameters query)`
- `Task ShareDocumentAsync(int documentId, DocumentShareRequest request)`
- `Task<DocumentAccessResult> GetDocumentAccessAsync(int documentId, int userId)`

### DTO Definitions

`DocumentUploadRequest`
- `string Title`
- `string Category`
- `string? Description`
- `int? ProjectId`
- `List<string>? Tags`
- `Stream FileStream`
- `string OriginalFileName`
- `string ContentType`
- `int UploadedByUserId`

`DocumentMetadataUpdate`
- `string Title`
- `string Category`
- `string? Description`
- `int? ProjectId`
- `List<string>? Tags`

`DocumentShareRequest`
- `int RecipientUserId`
- `string? RecipientTeam`
- `int SharedByUserId`
- `string AccessType`

`DocumentQueryParameters`
- `string? SearchText`
- `string? Category`
- `int? ProjectId`
- `DateTime? CreatedFrom`
- `DateTime? CreatedTo`
- `int PageSize`
- `int PageNumber`

`DocumentDto`
- `int DocumentId`
- `string Title`
- `string Category`
- `string? Description`
- `int? ProjectId`
- `string ProjectName`
- `int UploadedByUserId`
- `string UploadedByName`
- `DateTime UploadDate`
- `long FileSize`
- `string FileType`
- `bool IsPreviewable`
- `List<string> Tags`
- `bool IsSharedWithMe`

## UI Contracts

### Document Upload Page

Inputs:
- selected files
- title
- category
- optional project selector
- optional tags
- optional description

Outputs:
- success notification
- validation errors for unsupported file types and oversized files
- new document entries in the document list

### Document List Page

Features:
- sort by title, upload date, category, file size
- filter by category, project, date range
- search by title, description, tags, uploader, project
- actions: preview, download, edit metadata, replace file, delete, share

### Project Documents Section

Features:
- view all documents for the selected project
- allow project managers to upload and delete in project context
- preserve project association on upload

### Shared with Me Section

Features:
- list documents shared with the current user
- show share source and share date
- allow preview/download for shared documents

### Task Attachment Panel

Features:
- display current attachments for the task
- allow attachment of existing documents or upload of a new document from the task details page
- reflect task/project relationship automatically

## Notes

- The service contracts are intentionally decoupled from the Blazor UI.
- `IFileStorageService` isolates local filesystem operations to support future migration to alternate storage providers.
- UI contracts describe the shape and behavior of the pages without specifying Blazor component implementation details.
