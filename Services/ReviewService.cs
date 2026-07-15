using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Enums;

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

        public ReviewResponse MapToResponse(Review review)
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
            if (submission == null) throw new ArgumentException($"Submission {request.SubmissionId} not found.");

            Mentor? mentor = await _context.Mentors.FindAsync(request.MentorId);
            if (mentor == null) throw new ArgumentException($"Mentor {request.MentorId} not found.");

            Review review = new Review
            {
                SubmissionId = request.SubmissionId,
                MentorId = request.MentorId,
                Feedback = request.Feedback,
                Score = request.Score,
                ReviewStatus = Enum.Parse<ReviewStatus>(request.ReviewStatus!.ToString(), ignoreCase: true),
                ReviewedDate = DateTime.UtcNow,
                Mentor = mentor,
                Submission = submission
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Review Created Successfully with ID: {review.Id} at Timestamp: {DateTime.UtcNow}");
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
            if (review == null)
            {
                return null;
            }

            return MapToResponse(review);
        }
    }
}