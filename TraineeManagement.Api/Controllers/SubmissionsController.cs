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
    [Route("api/submissions")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;
        private readonly ILogger<SubmissionsController> _logger;

        public SubmissionsController(ISubmissionService submissionService, ILogger<SubmissionsController> logger)
        {
            _submissionService = submissionService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<SubmissionResponse>> CreateSubmission([FromBody] CreateSubmissionRequest request)
        {
            try
            {
                SubmissionResponse response = await _submissionService.CreateSubmissionAsync(request);
                _logger.LogInformation("Submission ID: {Id} created successfully.", response.Id);
                return CreatedAtAction(nameof(GetSubmissionById), new { id = response.Id }, response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Failed to create submission: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubmissionResponse>>> GetAllSubmissions()
        {
            IEnumerable<SubmissionResponse>? submissions = await _submissionService.GetAllSubmissionsAsync();
            return Ok(submissions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubmissionResponse>> GetSubmissionById(int id)
        {
            SubmissionResponse? submission = await _submissionService.GetSubmissionByIdAsync(id);
            if (submission == null)
            {
                _logger.LogWarning("Submission {Id} was not found.", id);
                return NotFound();
            }

            return submission;
        }
    }
}