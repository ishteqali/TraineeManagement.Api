using System.ComponentModel.DataAnnotations;
using TraineeManagement.Shared.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class CreateTaskAssignmentRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Trainee id is required")]
        public int TraineeId { get; set; }

        [Required(ErrorMessage = "Mentor id is required")]
        public int MentorId { get; set; }

        [Required(ErrorMessage = "Learning Task id is required")]
        public int LearningTaskId { get; set; }

        [Required(ErrorMessage = "Due Date is required")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(TaskAssignmentStatus), ErrorMessage = "Valid Status are Assigned, InProgress, Submitted, Revision or Completed")]
        public required string Status { get; set; }

        public string? Remarks { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DueDate < DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "The Due Date must be after the Assigned Date",
                    new[] { nameof(DueDate) }
                );
            }
        }
    }
}