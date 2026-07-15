using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
 
namespace TraineeManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;
 
        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }
 
        [HttpPost]
        public async Task<ActionResult<ReviewResponse>> CreateReview([FromBody] CreateReviewRequest request)
        {
            try
            {
                ReviewResponse response = await _reviewService.CreateReviewAsync(request);
                _logger.LogInformation("Review ID: {Id} created successfully.", response.Id);
                return CreatedAtAction(nameof(GetReviewById), new { id = response.Id }, response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Failed to create review: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetAllReviews()
        {
            IEnumerable<ReviewResponse> reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }
 
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewResponse>> GetReviewById(int id)
        {
            ReviewResponse? review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                _logger.LogWarning("Review {Id} was not found.", id);
                return NotFound();
            }
            
            return review;
        }
    }
}