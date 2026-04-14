namespace ContosoDashboard.Services;

public interface IVirusScanQueueService
{
    Task EnqueueVirusScanAsync(int documentId, string storagePath, string contentType, string originalFileName, int uploadedByUserId);
}
