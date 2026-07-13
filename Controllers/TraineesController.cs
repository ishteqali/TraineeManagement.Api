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
    [Route("api/[controller]")]
    public class TraineesController : ControllerBase
    {
        private readonly ITraineeService _traineeService;
        private readonly ILogger<TraineesController> _logger;

        public TraineesController(ITraineeService traineeService, ILogger<TraineesController> logger)
        {
            _traineeService = traineeService; // Dependency Injection
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<TraineeResponse>>> GetTrainees(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null
            )
        {

            PagedResponse<TraineeResponse>? trainees = await _traineeService.GetTraineesAsync(pageNumber, pageSize, search, status);
            return Ok(trainees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TraineeResponse>> GetById(int id)
        {
            TraineeResponse? trainee = await _traineeService.GetTraineeByIdAsync(id);
            if (trainee == null)
            {
                _logger.LogWarning($"Trainee with ID: {id} not found");
                return NotFound();
            }
            return Ok(trainee);
        }

        [HttpPost]
        public async Task<ActionResult<TraineeResponse>> Create([FromBody] CreateTraineeRequest request)
        {
            TraineeResponse createdTrainee = await _traineeService.AddTraineeAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = createdTrainee.Id }, createdTrainee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateTraineeRequest request)
        {
            TraineeResponse? updatedTrainee = await _traineeService.UpdateTraineeAsync(id, request);
            if (updatedTrainee == null)
            {
                _logger.LogWarning($"Trainee with ID: {id} not found");
                return NotFound();
            }
            return Ok(updatedTrainee);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool isDeleted = await _traineeService.DeleteTraineeAsync(id);
            if (!isDeleted)
            {
                _logger.LogWarning($"Trainee with ID: {id} not found");
                return NotFound();
            }
            return NoContent();
        }
    }
}
