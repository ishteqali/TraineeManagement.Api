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
        private readonly ILogger<ProcessingJobsController> _logger;

        public ProcessingJobsController(IProcessingJobService processingJobService, ILogger<ProcessingJobsController> logger)
        {
            _processingJobService = processingJobService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            ProcessingJobResponse job = await _processingJobService.GetByIdAsync(id, cancellationToken);
            return Ok(job);
        }
    }
}

