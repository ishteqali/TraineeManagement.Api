using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Enums;
using System.Collections.Generic;
namespace TraineeManagement.Api.DTOs
{
    public class UpdateLearningTaskRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(50, ErrorMessage = "Max 50 characters limit")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(100, ErrorMessage = "Max 100 characters limit")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Expected TechStack is required")]
        [MaxLength(100, ErrorMessage = "Max 100 characters limit")]
        public required string ExpectedTechStack { get; set; }

        [Required(ErrorMessage = "Due Date is required")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(LearningTaskStatus), ErrorMessage = "Valid Status are Draft, Published or Closed")]
        public required string Status { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DueDate <= DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Due date must be bigger than Creation date", new[] { nameof(DueDate) }
                );
            }
        }
    }
}