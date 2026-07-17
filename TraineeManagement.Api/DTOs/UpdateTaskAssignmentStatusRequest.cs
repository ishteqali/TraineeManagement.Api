using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class UpdateTaskAssignmentStatusRequest
    {
        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(TaskAssignmentStatus), ErrorMessage = "Valid Status are Assigned, InProgress, Submitted, Revision or Completed")]
        public required string Status { get; set; }

    }
}