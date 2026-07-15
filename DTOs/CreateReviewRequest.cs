using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class CreateReviewRequest
    {
        [Required(ErrorMessage = "Submission Id is required")]
        public int SubmissionId { get; set; }

        [Required(ErrorMessage = "Mentor Id is required")]
        public int MentorId { get; set; }

        [Required(ErrorMessage = "Feedback is required")]
        [MaxLength(100, ErrorMessage = "Max 100 characters limit")]
        public required string Feedback { get; set; }

        public int? Score { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(ReviewStatus), ErrorMessage = "Valid Status are Accepted, ChangesRequired or Rejected")]
        public required string ReviewStatus { get; set; }
    }
}