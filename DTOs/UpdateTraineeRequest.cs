using System.ComponentModel.DataAnnotations;
namespace TraineeManagement.Api.DTOs
{
    public class UpdateTraineeRequest
    {
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50, ErrorMessage = "Max 50 characters limit")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50, ErrorMessage = "Max 50 characters limit")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Valid Email Address is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tech Stack is required")]
        public string TechStack { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("(?i)^(Active|Inactive|Completed)", ErrorMessage = "Status must be either 'Active', 'Inactive' or 'Completed'")]
        public string Status { get; set; }
    }
}