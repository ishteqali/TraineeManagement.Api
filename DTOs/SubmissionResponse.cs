using System.Text.Json.Serialization;
using TraineeManagement.Api.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class SubmissionResponse
    {
        public int Id { get; set; }

        public int TaskAssignmentId { get; set; }
        public required string TaskTitle { get; set; }
        public required string SubmissionUrl { get; set; }
        public required string Notes { get; set; }
        public DateTime SubmittedDate { get; set; }
        public required string Status { get; set; }
    }
}