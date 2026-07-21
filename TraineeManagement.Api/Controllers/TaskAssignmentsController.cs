using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TraineeManagement.Api.DTOs;
using System.Threading.Tasks;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using TraineeManagement.Shared.Enums;

namespace TraineeManagement.Api.Controllers
{
    [Authorize(Roles = nameof(UserRole.Admin))]
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
            TaskAssignmentResponse response = await _taskAssignmentService.CreateAssignmentAsync(request);
            _logger.LogInformation("Task Assignment with ID: {Id} successfully created.", response.Id);
            return CreatedAtAction(nameof(GetAssignmentById), new { id = response.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskAssignmentResponse>>> GetAllAssignments()
        {
            IEnumerable<TaskAssignmentResponse> taskAssignments = await _taskAssignmentService.GetAllAssignmentAsync();
            return Ok(taskAssignments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskAssignmentResponse>> GetAssignmentById(int id)
        {
            TaskAssignmentResponse? taskAssignment = await _taskAssignmentService.GetAssignmentByIdAsync(id);
            if (taskAssignment is null)
            {
                _logger.LogWarning("Task Assignment Id: {id} not found", id);
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
                _logger.LogWarning("Task Assignment Id: {id} not found for updation of status", id);
                return NotFound(new { message = $"Task Assignment with ID: {id} was not found" });
            }
            return Ok(new { message = $"Task Assignment Status for ID: {id} updated successfully" });
        }
    }
}