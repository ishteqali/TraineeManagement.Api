using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TraineeManagement.Api.DTOs;
using System.Threading.Tasks;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/task-assignments")]
    public class TaskAssignmentsController : ControllerBase
    {
        private readonly ITaskAssignmentService _taskAssignmentService;
        private readonly ILogger<TaskAssignmentsController> _logger;
        public TaskAssignmentsController(ITaskAssignmentService taskAssignmentService, ILogger<TaskAssignmentsController> logger)
        {
            _taskAssignmentService = taskAssignmentService;
            _logger = logger;
        }
        [HttpPost]
        public async Task<ActionResult<TaskAssignmentResponse>> CreateAssignment([FromBody] CreateTaskAssignmentRequest request)
        {
            try
            {
                TaskAssignmentResponse response = await _taskAssignmentService.CreateAssignmentAsync(request);
                _logger.LogInformation($"Task Assignment with ID: {response.Id} successfully created.");
                return CreatedAtAction(nameof(GetAssignmentById), new { id = response.Id }, response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Failed to create assignment: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskAssignmentResponse>>> GetAllAssignments()
        {
            IEnumerable<TaskAssignmentResponse> taskAssignments = await _taskAssignmentService.GetAllAssignmentAsnyc();
            return Ok(taskAssignments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskAssignmentResponse>> GetAssignmentById(int id)
        {
            TaskAssignmentResponse? taskAssignment = await _taskAssignmentService.GetAssignmentByIdAsync(id);
            if (taskAssignment == null)
            {
                _logger.LogWarning($"Task Assignment Id: {id} not found");
                return NotFound();
            }
            return Ok(taskAssignment);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateTaskAssignmentStatusRequest request)
        {
            bool success = await _taskAssignmentService.UpdateStatusAsync(id, request.Status);
            if (!success)
            {
                _logger.LogWarning($"Task Assignment Id: {id} not found for updation of status");
                return NotFound(new { message = $"Task Assignment with ID: {id} was not found" });
            }
            return Ok(new { message = $"Task Assignment Status for ID: {id} updated successfully" });
        }
    }
}