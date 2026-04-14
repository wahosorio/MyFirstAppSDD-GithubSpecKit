using System.Text;

namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _baseDirectory;

    public LocalFileStorageService(IConfiguration configuration)
    {
        var baseDirectory = configuration.GetValue<string>("FileStorage:BaseDirectory") ?? "AppData/uploads";
        _baseDirectory = Path.IsPathRooted(baseDirectory)
            ? baseDirectory
            : Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), baseDirectory));

        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
        }
    }

    public async Task<string> UploadAsync(Stream fileStream, string userId, string projectId, string fileName, string contentType)
    {
        var safeProjectId = string.IsNullOrWhiteSpace(projectId) ? "personal" : projectId;
        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = Path.GetExtension(fileName);
        }

        var relativeDirectory = Path.Combine(userId, safeProjectId);
        var relativeFileName = $"{Guid.NewGuid()}{extension}";
        var relativePath = Path.Combine(relativeDirectory, relativeFileName);
        var absolutePath = GetAbsolutePath(relativePath);

        var directory = Path.GetDirectoryName(absolutePath);
        if (directory == null)
        {
            throw new InvalidOperationException("Unable to determine file directory for storage.");
        }

        Directory.CreateDirectory(directory);

        using var targetStream = File.Create(absolutePath);
        await fileStream.CopyToAsync(targetStream);

        return relativePath.Replace(Path.DirectorySeparatorChar, '/');
    }

    public Task<Stream> OpenReadAsync(string storagePath)
    {
        var absolutePath = GetAbsolutePath(storagePath);
        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException($"Stored file not found: {absolutePath}");
        }

        return Task.FromResult<Stream>(File.OpenRead(absolutePath));
    }

    public Task DeleteAsync(string storagePath)
    {
        var absolutePath = GetAbsolutePath(storagePath);
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string storagePath)
    {
        var absolutePath = GetAbsolutePath(storagePath);
        return Task.FromResult(File.Exists(absolutePath));
    }

    public string GetAbsolutePath(string storagePath)
    {
        if (Path.IsPathRooted(storagePath))
        {
            return storagePath;
        }

        var combined = Path.GetFullPath(Path.Combine(_baseDirectory, storagePath.Replace('/', Path.DirectorySeparatorChar)));
        if (!combined.StartsWith(_baseDirectory, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Invalid storage path.");
        }

        return combined;
    }
}
