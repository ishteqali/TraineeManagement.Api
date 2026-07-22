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
    private const string RootDirectory = "../Upload";

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
            ProcessingJob? processingJob = await FetchProcessingJobAsync(message, cancellationToken);

            if (processingJob is null || processingJob.Status is ProcessingStatus.Completed)
            {
                return ProcessingResultStatus.Success;
            }

            await UpdateJobToProcessingAsync(processingJob, cancellationToken);
            _logger.LogInformation("Job {id} is Processing.", processingJob.Id);

            SubmissionFile? submissionFile = await FetchSubmissionFileOrThrowAsync(processingJob.SubmissionFileId, cancellationToken);
            _logger.LogInformation("Submission file {id} found for Processing", processingJob.SubmissionFileId);

            string filePath = Path.Combine(RootDirectory, submissionFile.StorageFileName);
            submissionFile.Checksum = await CalculateChecksumAsync(filePath, cancellationToken);

            await CompleteJobAsync(processingJob, cancellationToken);
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
            .FirstOrDefaultAsync(processingJob => processingJob.MessageId == message.MessageId, cancellationToken);

        if (processingJob is null)
        {
            return ProcessingResultStatus.DeadLetter;
        }

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
            throw new FileNotFoundException("File not found: {filePath}", filePath);
        }

        await using FileStream stream = File.OpenRead(filePath);

        byte[] hash = await SHA256.HashDataAsync(stream, cancellationToken);

        return Convert.ToHexString(hash);
    }

    private async Task<ProcessingJob?> FetchProcessingJobAsync(SubmissionProcessingRequested message, CancellationToken token)
    {
        ProcessingJob? job = await _context.ProcessingJobs.FirstOrDefaultAsync(
            currentProcessingJob => currentProcessingJob.MessageId == message.MessageId ||
            currentProcessingJob.SubmissionFileId == message.SubmissionFileId, token);

        if (job is null)
        {
            _logger.LogWarning("ProcessingJob not found.");
        }
        else if (job.Status is ProcessingStatus.Completed)
        {
            _logger.LogInformation("Duplicate message ignored.");
        }

        return job;
    }

    private async Task UpdateJobToProcessingAsync(ProcessingJob job, CancellationToken token)
    {
        job.Status = ProcessingStatus.Processing;
        job.StartedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(token);
    }

    private async Task<SubmissionFile> FetchSubmissionFileOrThrowAsync(int fileId, CancellationToken token)
    {
        SubmissionFile? file = await _context.SubmissionFiles.FirstOrDefaultAsync(currentSubmissionFile => currentSubmissionFile.Id == fileId, token);
        return file ?? throw new FileNotFoundException("Submission file entry not found in the database.");
    }

    private async Task CompleteJobAsync(ProcessingJob job, CancellationToken token)
    {
        job.Status = ProcessingStatus.Completed;
        job.CompletedAt = DateTime.UtcNow;
        job.ErrorSummary = null;
        await _context.SaveChangesAsync(token);
    }
}