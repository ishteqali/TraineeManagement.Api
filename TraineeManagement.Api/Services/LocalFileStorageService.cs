using Microsoft.Extensions.Options;
using TraineeManagement.Api.Configurations;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Constants;
using TraineeManagement.Api.Exceptions;

namespace TraineeManagement.Api.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private const int BufferSize = 81920;
        private readonly FileStorageOptions _options;

        public LocalFileStorageService(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
        }

        public async Task<string> SaveAsync(Stream fileStream, string fileExtension, CancellationToken cancellationToken = default)
        {
            string? storageFileName = $"{Guid.NewGuid()}{fileExtension}";

            Directory.CreateDirectory(_options.RootPath);

            string? filePath = GetFilePath(storageFileName);

            await using FileStream? outputStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                BufferSize,
                useAsync: true);

            await fileStream.CopyToAsync(outputStream, cancellationToken);

            return storageFileName;
        }

        public Task<bool> ExistsAsync(string storageFileName, CancellationToken cancellationToken = default)
        {
            string? filePath = GetFilePath(storageFileName);

            return Task.FromResult(File.Exists(filePath));
        }

        public Task DeleteAsync(string storageFileName, CancellationToken cancellationToken = default)
        {
            string? filePath = GetFilePath(storageFileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }

        public Task<Stream> OpenReadAsync(string storageFileName, CancellationToken cancellationToken = default)
        {
            string? filePath = GetFilePath(storageFileName);

            if (!File.Exists(filePath))
            {
                throw new NotFoundException(ExceptionMessages.FileNotFound);
            }

            Stream stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                BufferSize,
                useAsync: true);

            return Task.FromResult(stream);
        }
        private string GetFilePath(string storageFileName)
        {
            return Path.Combine(_options.RootPath, storageFileName);
        }
    }
}

