using Microsoft.EntityFrameworkCore;
using TraineeManagement.Shared.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Shared.Models;
using TraineeManagement.Api.Configurations;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Constants;
using TraineeManagement.Api.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using TraineeManagement.Shared.Contracts;
using TraineeManagement.Shared.Enums;

namespace TraineeManagement.Api.Services
{
    public class SubmissionFileService : ISubmissionFileService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly FileStorageOptions _options;
        private readonly ILogger<SubmissionFileService> _logger;
        private readonly IMessagePublisher _messagePublisher;

        public SubmissionFileService(AppDbContext context, IFileStorageService fileStorageService,
            ILogger<SubmissionFileService> logger, IOptions<FileStorageOptions> options, IMessagePublisher messagePublisher)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _logger = logger;
            _options = options.Value;
            _messagePublisher = messagePublisher;
        }

        private const int BytesPerUnit = 1024;

        public async Task<SubmissionFileResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            SubmissionFile? submissionFile = await _context.SubmissionFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(subFile => subFile.Id == id, cancellationToken);

            if (submissionFile is null)
            {
                throw new NotFoundException(ExceptionMessages.SubmissionFileNotFound(id));
            }
            _logger.LogInformation("Retrieved submission file with Id: {id}.", id);

            return MapToResponse(submissionFile);
        }

        public async Task<SubmissionAcceptedResponse> UploadAsync(int submissionId, UploadSubmissionFileRequest request,
            int uploadedBy, CancellationToken cancellationToken = default)
        {
            await ValidateSubmissionExistsAsync(submissionId, cancellationToken);

            ValidateFile(request.File);

            string? extension = Path.GetExtension(request.File.FileName);

            string? storageFileName = await _fileStorageService.SaveAsync(
                request.File.OpenReadStream(),
                extension,
                cancellationToken);
            SubmissionFile submissionFile;
            try
            {
                submissionFile = CreateSubmissionFile(submissionId, request, storageFileName, uploadedBy);

                _context.SubmissionFiles.Add(submissionFile);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await _fileStorageService.DeleteAsync(storageFileName, cancellationToken);
                throw;
            }
            SubmissionProcessingRequested message = new()
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = Guid.NewGuid(),
                SubmissionId = submissionId,
                FileId = submissionFile.Id,
                RequestedAt = DateTime.UtcNow,
                ContractVersion = "1.0"
            };

            ProcessingJob processingJob = new()
            {
                SubmissionId = submissionId,
                SubmissionFileId = submissionFile.Id,
                MessageId = message.MessageId,
                CorrelationId = message.CorrelationId,
                Status = ProcessingStatus.Queued,
                Attempts = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProcessingJobs.Add(processingJob);

            await _context.SaveChangesAsync(cancellationToken);

            bool published = await _messagePublisher.PublishSubmissionProcessingAsync(message, cancellationToken);

            if (!published)
            {
                processingJob.Status = ProcessingStatus.Failed;
                processingJob.ErrorSummary = "Failed to publish message to RabbitMQ";
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogError("RabbitMQ publish failed. SubmissionId: {submissionId}", submissionId);
                throw new BadRequestException(ExceptionMessages.RabbitMQPublishFailed);
            }

            _logger.LogInformation("Submission queued successfully. MessageId: {MessageId}, CorrelationId: {CorrelationId}, SubmissionId: {SubmissionId}",
                message.MessageId, message.CorrelationId, submissionId);

            return new SubmissionAcceptedResponse
            {
                TrackingId = message.CorrelationId,
                Message = "Submission accepted for processing."
            };
        }

        public async Task<DownloadSubmissionFileResponse> DownloadAsync(int id, CancellationToken cancellationToken = default)
        {
            SubmissionFile? submissionFile = await _context.SubmissionFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    subFile => subFile.Id == id,
                    cancellationToken);

            if (submissionFile is null)
            {
                throw new NotFoundException(ExceptionMessages.SubmissionFileNotFound(id));
            }

            Stream? stream = await _fileStorageService.OpenReadAsync(submissionFile.StorageFileName, cancellationToken);

            return new DownloadSubmissionFileResponse
            {
                Stream = stream,
                ContentType = submissionFile.ContentType,
                FileName = submissionFile.OriginalFileName
            };
        }
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            SubmissionFile? submissionFile = await _context.SubmissionFiles
                .FirstOrDefaultAsync(
                    subFile => subFile.Id == id,
                    cancellationToken);

            if (submissionFile is null)
            {
                throw new NotFoundException(ExceptionMessages.SubmissionFileNotFound(id));
            }

            await _fileStorageService.DeleteAsync(submissionFile.StorageFileName, cancellationToken);

            _context.SubmissionFiles.Remove(submissionFile);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Submission file {id} deleted.", id);
        }

        private static SubmissionFileResponse MapToResponse(SubmissionFile submissionFile)
        {
            return new SubmissionFileResponse
            {
                Id = submissionFile.Id,
                OriginalFileName = submissionFile.OriginalFileName,
                ContentType = submissionFile.ContentType,
                FileSize = submissionFile.FileSize,
                UploadedDate = submissionFile.UploadedDate
            };
        }

        private async Task ValidateSubmissionExistsAsync(int submissionId, CancellationToken cancellationToken)
        {
            Submission? submission = await _context.Submissions
                .FirstOrDefaultAsync(
                    s => s.Id == submissionId,
                    cancellationToken);

            if (submission is null)
            {
                throw new NotFoundException(ExceptionMessages.SubmissionNotFound(submissionId));
            }
        }
        private void ValidateFile(IFormFile file)
        {
            if (file is null)
            {
                throw new BadRequestException(ExceptionMessages.EmptyFile);
            }
            if (file.Length == 0)
            {
                throw new BadRequestException(ExceptionMessages.EmptyFile);
            }
            string? extension = Path.GetExtension(file.FileName);

            if (!_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new BadRequestException(ExceptionMessages.InvalidFileType);
            }
            if (file.Length > _options.MaxFileSizeInMB * BytesPerUnit * BytesPerUnit)
            {
                throw new BadRequestException(ExceptionMessages.FileTooLarge);
            }
        }

        private SubmissionFile CreateSubmissionFile(int submissionId, UploadSubmissionFileRequest request,
            string storageFileName, int uploadedBy)
        {
            return new SubmissionFile
            {
                SubmissionId = submissionId,
                OriginalFileName = request.File.FileName,
                StorageFileName = storageFileName,
                ContentType = request.File.ContentType,
                FileSize = request.File.Length,
                UploadedBy = uploadedBy,
                UploadedDate = DateTime.UtcNow
            };
        }
    }
}

