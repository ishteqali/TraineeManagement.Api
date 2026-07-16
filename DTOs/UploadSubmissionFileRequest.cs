using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TraineeManagement.Api.DTOs
{
    public class UploadSubmissionFileRequest
    {
        [Required(ErrorMessage = "Please select a file.")]
        public IFormFile File { get; set; } = null!;
    }
}

