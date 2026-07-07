using System.Data.Common;
using TraineeManagement.Api.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class TraineeResponse
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string TechStack { get; set; }
        public required string Status { get; set; }
    }
}