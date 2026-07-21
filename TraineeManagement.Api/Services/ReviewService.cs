using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Shared.Models;
using TraineeManagement.Shared.Data;
using TraineeManagement.Shared.Enums;
using TraineeManagement.Api.Constants;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.Api.Helpers;

namespace TraineeManagement.Api.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(AppDbContext context, ILogger<ReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private ReviewResponse MapToResponse(Review review)
        {
            return new ReviewResponse
            {
                Id = review.Id,
                SubmissionId = review.SubmissionId,
                SubmissionUrl = review.Submission.SubmissionUrl,
                MentorId = review.MentorId,
                MentorName = $"{review.Mentor.FirstName} {review.Mentor.LastName}".Trim(),
                Feedback = review.Feedback,
                Score = review.Score,
                ReviewStatus = review.ReviewStatus.ToString(),
                ReviewedDate = review.ReviewedDate
            };
        }
        public async Task<ReviewResponse> CreateReviewAsync(CreateReviewRequest request)
        {
            Submission? submission = await _context.Submissions.FindAsync(request.SubmissionId);
            if (submission is null) throw new NotFoundException(ExceptionMessages.SubmissionNotFound(request.SubmissionId));

            Mentor? mentor = await _context.Mentors.FindAsync(request.MentorId);
            if (mentor is null) throw new NotFoundException(ExceptionMessages.MentorNotFound(request.MentorId));

            Review review = new Review
            {
                SubmissionId = request.SubmissionId,
                MentorId = request.MentorId,
                Feedback = request.Feedback,
                Score = request.Score,
                ReviewStatus = EnumHelper.ParseOrThrow<ReviewStatus>(request.ReviewStatus, nameof(request.ReviewStatus)),
                ReviewedDate = DateTime.UtcNow,
                Mentor = mentor,
                Submission = submission
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Review Created Successfully with ID: {ReviewId} at Timestamp: {Timestamp}",
                review.Id, DateTime.UtcNow);

            return MapToResponse(review);
        }

        public async Task<IEnumerable<ReviewResponse>> GetAllReviewsAsync()
        {
            IEnumerable<Review> reviews = await _context.Reviews
                .Include(review => review.Submission)
                .Include(review => review.Mentor)
                .ToListAsync();

            return reviews.Select(MapToResponse).ToList();
        }

        public async Task<ReviewResponse?> GetReviewByIdAsync(int id)
        {
            Review? review = await _context.Reviews
                .Include(review => review.Submission)
                .Include(review => review.Mentor)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (review is null)
            {
                return null;
            }

            return MapToResponse(review);
        }
    }
}