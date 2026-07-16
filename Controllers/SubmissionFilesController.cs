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
    [Authorize]
    [ApiController]
    [Route("api/")]
    public class SubmissionFilesController : ControllerBase
    {
        private readonly ISubmissionFileService _submissionFileService;
        private readonly ILogger<SubmissionFilesController> _logger;
        public SubmissionFilesController(ISubmissionFileService submissionFileService, ILogger<SubmissionFilesController> logger)
        {
            _submissionFileService = submissionFileService;
            _logger = logger;
        }
        [HttpPost("submissions/{submissionId:int}/files")]
        public async Task<ActionResult<SubmissionFileResponse>> Upload(int submissionId, [FromForm] UploadSubmissionFileRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reached in Controller");
            int uploadedBy = 1; // Temporary

            SubmissionFileResponse response = await _submissionFileService.UploadAsync(submissionId, request, uploadedBy, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpGet("submissions/{id:int}/files")]
        public async Task<ActionResult<SubmissionFileResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            SubmissionFileResponse response = await _submissionFileService.GetByIdAsync(id, cancellationToken);
            return Ok(response);
        }

        [HttpGet("submission-files/{id:int}/download")]
        public async Task<IActionResult> Download(int id, CancellationToken cancellationToken)
        {
            DownloadSubmissionFileResponse? response = await _submissionFileService.DownloadAsync(id, cancellationToken);

            return File(
                response.Stream,
                response.ContentType,
                response.FileName);
        }
        [HttpDelete("submission-files/{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _submissionFileService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}