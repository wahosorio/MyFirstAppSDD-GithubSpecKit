using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;

namespace ContosoDashboard.Services;

public class AzureQueueVirusScanService : IVirusScanQueueService
{
    private readonly string? _connectionString;
    private readonly string? _queueName;
    private readonly string _localQueueDirectory;

    public AzureQueueVirusScanService(IConfiguration configuration)
    {
        _connectionString = configuration.GetValue<string>("VirusScanQueue:ConnectionString");
        _queueName = configuration.GetValue<string>("VirusScanQueue:QueueName");
        _localQueueDirectory = configuration.GetValue<string>("VirusScanQueue:LocalQueueDirectory") ?? "AppData/queue";

        if (!Directory.Exists(_localQueueDirectory))
        {
            Directory.CreateDirectory(_localQueueDirectory);
        }
    }

    public async Task EnqueueVirusScanAsync(int documentId, string storagePath, string contentType, string originalFileName, int uploadedByUserId)
    {
        var payload = new
        {
            DocumentId = documentId,
            StoragePath = storagePath,
            ContentType = contentType,
            OriginalFileName = originalFileName,
            UploadedByUserId = uploadedByUserId,
            EnqueuedAt = DateTime.UtcNow
        };

        var content = JsonSerializer.Serialize(payload);

        if (!string.IsNullOrWhiteSpace(_connectionString) && !string.IsNullOrWhiteSpace(_queueName))
        {
            var queueClient = new QueueClient(_connectionString, _queueName);
            await queueClient.CreateIfNotExistsAsync();
            var base64Message = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
            await queueClient.SendMessageAsync(base64Message);
            return;
        }

        var fileName = $"{documentId}_{Guid.NewGuid()}.json";
        var path = Path.Combine(_localQueueDirectory, fileName);
        await File.WriteAllTextAsync(path, content);
    }
}
