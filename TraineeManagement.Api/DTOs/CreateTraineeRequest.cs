using System.ComponentModel.DataAnnotations;
using TraineeManagement.Shared.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class CreateTraineeRequest
    {
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50, ErrorMessage = "Max 50 characters limit")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50, ErrorMessage = "Max 50 characters limit")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Valid Email Address is required")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Tech Stack is required")]
        public required string TechStack { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(TraineeStatus), ErrorMessage = "Valid Status are Active, Inactive or Completed")]
        public required string Status { get; set; }
    }
}