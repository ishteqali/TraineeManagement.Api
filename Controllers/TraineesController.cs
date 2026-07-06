using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Services;

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
        public ActionResult<List<TraineeResponse>> GetAll()
        {   
            var trainees = _traineeService.GetAllTrainees();
            return Ok(trainees);
        }

        [HttpGet("{id}")]
        public ActionResult<TraineeResponse> GetById(int id)
        {
            var trainee = _traineeService.GetTraineeById(id);
            if(trainee == null)
            {
                return NotFound();
            }
            return Ok(trainee);
        }

        [HttpPost]
        public ActionResult<TraineeResponse> Create([FromBody] CreateTraineeRequest request)
        {
            var createdTrainee = _traineeService.AddTrainee(request);

            return CreatedAtAction(nameof(GetById), new { id = createdTrainee.Id }, createdTrainee);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, UpdateTraineeRequest request)
        {
            var isUpdated = _traineeService.UpdateTrainee(id, request);
            if(!isUpdated)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var isDeleted = _traineeService.DeleteTrainee(id);
            if(!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
