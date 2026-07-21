using System.Text.Json.Serialization;
using TraineeManagement.Shared.Enums;
namespace TraineeManagement.Shared.Models
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

        [JsonIgnore]
        public ICollection<SubmissionFile> Files = new List<SubmissionFile>();

        [JsonIgnore]
        public ICollection<ProcessingJob> ProcessingJobs { get; set; } = new List<ProcessingJob>();
    }
}