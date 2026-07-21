using Microsoft.EntityFrameworkCore;
using TraineeManagement.Shared.Data;
using TraineeManagement.Shared.Contracts;
using TraineeManagement.Shared.Enums;
using TraineeManagement.Shared.Models;
using TraineeManagement.Worker.Interfaces;
using System.Security.Cryptography;

namespace TraineeManagement.Worker.Services;

public class SubmissionProcessorService : ISubmissionProcessorService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionProcessorService> _logger;

    private const int MaxRetryAttempts = 3;

    public SubmissionProcessorService(AppDbContext context, ILogger<SubmissionProcessorService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<ProcessingResultStatus> ProcessAsync(SubmissionProcessingRequested message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing message: {MessageId}", message.MessageId);
        try
        {
            ProcessingJob? processingJob =
                await _context.ProcessingJobs
                .FirstOrDefaultAsync(
                p => p.MessageId == message.MessageId || p.SubmissionFileId == message.FileId,
                cancellationToken);

            if (processingJob == null)
            {
                _logger.LogWarning("ProcessingJob not found.");

                return ProcessingResultStatus.Success;
            }
            if (processingJob.Status == ProcessingStatus.Completed)
            {
                _logger.LogInformation("Duplicate message ignored.");

                return ProcessingResultStatus.Success;
            }

            processingJob.Status = ProcessingStatus.Processing;
            processingJob.StartedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            SubmissionFile? submissionFile = await _context.SubmissionFiles
                .FirstOrDefaultAsync(
                s => s.Id == processingJob.SubmissionFileId,
                cancellationToken);

            if (submissionFile == null)
            {
                throw new FileNotFoundException("Submission file not found.");
            }
            string filePath = Path.Combine("../Uploads", submissionFile.StorageFileName);

            submissionFile.Checksum = await CalculateChecksumAsync(filePath, cancellationToken);


            processingJob.Status = ProcessingStatus.Completed;
            processingJob.CompletedAt = DateTime.UtcNow;
            processingJob.ErrorSummary = null;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Submission {submissionId} processed successfully.", message.SubmissionId);
            return ProcessingResultStatus.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message {MessageId}", message.MessageId);

            return await HandleFailureAsync(message, ex, cancellationToken);
        }
    }
    private async Task<ProcessingResultStatus> HandleFailureAsync(SubmissionProcessingRequested message, Exception exception,
        CancellationToken cancellationToken)
    {
        ProcessingJob? processingJob = await _context.ProcessingJobs
            .FirstOrDefaultAsync(
                p => p.MessageId == message.MessageId,
                cancellationToken);

        if (processingJob == null)
            return ProcessingResultStatus.DeadLetter;

        processingJob.Attempts++;

        processingJob.ErrorSummary = exception.Message;

        if (processingJob.Attempts >= MaxRetryAttempts)
        {
            processingJob.Status = ProcessingStatus.Failed;

            processingJob.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ProcessingResultStatus.DeadLetter;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return ProcessingResultStatus.Retry;
    }

    private async Task<string> CalculateChecksumAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        await using FileStream stream = File.OpenRead(filePath);

        byte[] hash = await SHA256.HashDataAsync(stream, cancellationToken);

        return Convert.ToHexString(hash);
    }
}