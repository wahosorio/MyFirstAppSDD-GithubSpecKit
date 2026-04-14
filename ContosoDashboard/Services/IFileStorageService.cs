namespace ContosoDashboard.Services;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string userId, string projectId, string fileName, string contentType);
    Task<Stream> OpenReadAsync(string storagePath);
    Task DeleteAsync(string storagePath);
    Task<bool> ExistsAsync(string storagePath);
    string GetAbsolutePath(string storagePath);
}
