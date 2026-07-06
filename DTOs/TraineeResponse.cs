using System.Data.Common;

namespace TraineeManagement.Api.DTOs
{
    public class TraineeResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string TechStack { get; set; }
        public string Status { get; set; }
    }
}