using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace TraineeManagement.Api.Helpers
{
    public static class FileHelper
    {
        public static async Task<string> CalculateChecksumAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}", filePath);
            }

            await using FileStream stream = File.OpenRead(filePath);

            byte[] hash = await SHA256.HashDataAsync(stream, cancellationToken);

            return Convert.ToHexString(hash);
        }
    }
}