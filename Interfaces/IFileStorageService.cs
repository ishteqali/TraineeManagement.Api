using System.IO;
namespace TraineeManagement.Api.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream fileStream, string fileExtension, CancellationToken cancellationToken);

        Task<Stream> OpenReadAsync(string storageFileName, CancellationToken cancellationToken);

        Task<bool> ExistsAsync(string storageFileName, CancellationToken cancellationToken);

        Task DeleteAsync(string storageFileName, CancellationToken cancellationToken);
    }
}