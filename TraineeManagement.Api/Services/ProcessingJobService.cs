using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Shared.Data;
using TraineeManagement.Shared.Models;
namespace TraineeManagement.Api.Services
{
    public class ProcessingJobService : IProcessingJobService
    {
        private readonly AppDbContext _context;

        public ProcessingJobService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProcessingJobResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            ProcessingJob? job = await _context.ProcessingJobs
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == id,
                    cancellationToken);

            if (job == null)
            {
                throw new NotFoundException($"Processing Job {id} not found.");
            }

            return new ProcessingJobResponse
            {
                Id = job.Id,
                SubmissionId = job.SubmissionId,
                FileId = job.SubmissionFileId,
                Status = job.Status.ToString(),
                Attempts = job.Attempts,
                ErrorSummary = job.ErrorSummary,
                CreatedAt = job.CreatedAt,
                StartedAt = job.StartedAt,
                CompletedAt = job.CompletedAt,
                CorrelationId = job.CorrelationId
            };
        }
    }
}
