using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/processing-jobs")]
    public class ProcessingJobsController : ControllerBase
    {
        private readonly IProcessingJobService _processingJobService;

        public ProcessingJobsController(IProcessingJobService processingJobService)
        {
            _processingJobService = processingJobService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            ProcessingJobResponse job = await _processingJobService.GetByIdAsync(id, cancellationToken);
            return Ok(job);
        }
    }
}

