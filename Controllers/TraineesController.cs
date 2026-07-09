using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TraineeManagement.Api.DTOs;
using System.Threading.Tasks;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineesController : ControllerBase
    {
        private readonly ITraineeService _traineeService;

        public TraineesController(ITraineeService traineeService)
        {
            _traineeService = traineeService; // Dependency Injection
        }

        [HttpGet]
        public async Task<ActionResult<List<TraineeResponse>>> GetAll([FromQuery] string? search = null)
        {
            List<TraineeResponse> trainees = await _traineeService.GetAllTraineesAsync(search);
            return Ok(trainees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TraineeResponse>> GetById(int id)
        {
            TraineeResponse? trainee = await _traineeService.GetTraineeByIdAsync(id);
            if (trainee == null)
            {
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
            bool isUpdated = await _traineeService.UpdateTraineeAsync(id, request);
            if (!isUpdated)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool isDeleted =await  _traineeService.DeleteTraineeAsync(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
