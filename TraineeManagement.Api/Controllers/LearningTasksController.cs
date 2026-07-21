using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TraineeManagement.Api.DTOs;
using System.Threading.Tasks;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagementApi.Controllers
{
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiController]
    [Route("api/learning-tasks")]
    public class LearningTasksController : ControllerBase
    {
        private readonly ILearningTaskService _learningTaskService;
        private readonly ILogger<LearningTasksController> _logger;

        public LearningTasksController(ILearningTaskService learningTaskService, ILogger<LearningTasksController> logger)
        {
            _learningTaskService = learningTaskService; // Dependency Injection
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<LearningTaskResponse>>> GetLearningTasks(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null
            )
        {

            PagedResponse<LearningTaskResponse>? learningTasks = await _learningTaskService.GetLearningTasksAsync(pageNumber, pageSize, search, status);
            if (learningTasks is null)
            {
                _logger.LogInformation("Unable to fetch any Learning Tasks");
            }
            return Ok(learningTasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LearningTaskResponse>> GetById(int id)
        {
            LearningTaskResponse? learningTask = await _learningTaskService.GetLearningTaskByIdAsync(id);
            if (learningTask is null)
            {
                _logger.LogWarning("Learning Task with ID: {id} not found", id);
                return NotFound();
            }
            return Ok(learningTask);
        }

        [HttpPost]
        public async Task<ActionResult<LearningTaskResponse>> Create([FromBody] CreateLearningTaskRequest request)
        {
            LearningTaskResponse createdLearningTask = await _learningTaskService.AddLearningTaskAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = createdLearningTask.Id }, createdLearningTask);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateLearningTaskRequest request)
        {
            LearningTaskResponse? updatedLearningTask = await _learningTaskService.UpdateLearningTaskAsync(id, request);
            if (updatedLearningTask is null)
            {
                _logger.LogWarning("Learning Task with ID: {id} not found", id);
                return NotFound();
            }
            return Ok(updatedLearningTask);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool isDeleted = await _learningTaskService.DeleteLearningTaskAsync(id);
            if (!isDeleted)
            {
                _logger.LogWarning("Learning Task with ID: {id} not found", id);
                return NotFound();
            }
            return NoContent();
        }
    }
}
