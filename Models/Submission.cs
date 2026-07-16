using System.Text.Json.Serialization;
using TraineeManagement.Api.Enums;
namespace TraineeManagement.Api.Models
{
    public class Submission
    {
        public int Id { get; set; }

        public int TaskAssignmentId { get; set; }

        [JsonIgnore]
        public required TaskAssignment TaskAssignment { get; set; }
        public required string SubmissionUrl { get; set; }
        public required string Notes { get; set; }
        public DateTime SubmittedDate { get; set; }
        public SubmissionStatus Status { get; set; }
        public ICollection<SubmissionFile> Files = new List<SubmissionFile>();
    }
}