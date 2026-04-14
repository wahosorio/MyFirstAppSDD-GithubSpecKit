using System.Text;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DocumentVirusScanFunctionApp;

public static class DocumentVirusScanFunction
{
    [FunctionName("DocumentVirusScanFunction")]
    public static async Task Run(
        [QueueTrigger("documentscans", Connection = "VirusScanQueue:ConnectionString")] string queueMessage,
        ILogger log)
    {
        log.LogInformation("Virus scan request received.");

        var messageBytes = Convert.FromBase64String(queueMessage);
        var payloadJson = Encoding.UTF8.GetString(messageBytes);
        var payload = JsonSerializer.Deserialize<VirusScanRequest>(payloadJson);

        if (payload == null)
        {
            log.LogWarning("Queue payload was empty or invalid.");
            return;
        }

        log.LogInformation("Scanning file {DocumentId} stored at {StoragePath}.", payload.DocumentId, payload.StoragePath);

        // TODO: Replace with actual antivirus engine or scanner integration.
        await Task.Delay(500);

        log.LogInformation("Document {DocumentId} marked as safe after scan.", payload.DocumentId);

        // In a full implementation, update the document record in the database or send a callback event.
    }
}

public class VirusScanRequest
{
    public int DocumentId { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public int UploadedByUserId { get; set; }
    public DateTime EnqueuedAt { get; set; }
}
