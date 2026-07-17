using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class CreateSubmissionRequest
    {
        [Required(ErrorMessage = "Task Assignment ID is required")]
        public int TaskAssignmentId { get; set; }

        [Required(ErrorMessage = "A valid Github or Drive link is required")]
        [Url]
        public required string SubmissionUrl { get; set; }

        [Required(ErrorMessage = "Notes is required")]
        [MaxLength(100, ErrorMessage = "Max 100 characters limit")]
        public required string Notes { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(SubmissionStatus), ErrorMessage = "Valid Status are Submitted or Resubmitted")]
        public required string Status { get; set; }
    }
}