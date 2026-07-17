using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TraineeManagement.Api.DTOs;
using System.Threading.Tasks;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagementApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/mentors")]
    public class MentorsController : ControllerBase
    {
        private readonly IMentorService _mentorService;
        private readonly ILogger<MentorsController> _logger;

        public MentorsController(IMentorService mentorService, ILogger<MentorsController> logger)
        {
            _mentorService = mentorService; // Dependency Injection
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<MentorResponse>>> GetMentors(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null
            )
        {

            PagedResponse<MentorResponse>? mentors = await _mentorService.GetMentorsAsync(pageNumber, pageSize, search, status);
            return Ok(mentors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MentorResponse>> GetById(int id)
        {
            MentorResponse? mentor = await _mentorService.GetMentorByIdAsync(id);
            if (mentor == null)
            {
                _logger.LogWarning($"Mentor with ID: {id} not found");
                return NotFound();
            }
            return Ok(mentor);
        }

        [HttpPost]
        public async Task<ActionResult<MentorResponse>> Create([FromBody] CreateMentorRequest request)
        {
            MentorResponse createdMentor = await _mentorService.AddMentorAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = createdMentor.Id }, createdMentor);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateMentorRequest request)
        {
            MentorResponse? updatedMentor = await _mentorService.UpdateMentorAsync(id, request);
            if (updatedMentor == null)
            {
                _logger.LogWarning($"Mentor with ID: {id} not found");
                return NotFound();
            }
            return Ok(updatedMentor);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool isDeleted = await _mentorService.DeleteMentorAsync(id);
            if (!isDeleted)
            {
                _logger.LogWarning($"Mentor with ID: {id} not found");
                return NotFound();
            }
            return NoContent();
        }
    }
}
