using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class UpdateMentorRequest
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

        [Required(ErrorMessage = "Expertise is required")]
        public required string Expertise { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(MentorStatus), ErrorMessage = "Valid Status are Active or Inactive")]
        public required string Status { get; set; }
    }
}