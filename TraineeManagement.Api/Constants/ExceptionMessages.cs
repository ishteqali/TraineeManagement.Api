namespace TraineeManagement.Api.Constants
{
    public static class ExceptionMessages
    {
        // Trainee
        public static string TrianeeNotFound(int id)
        {
            return $"Trianee with Id '{id}' was not found.";
        }

        // Mentor
        public static string MentorNotFound(int id)
        {
            return $"Mento with Id '{id}' was not found.";
        }

        // Learning task
        public static string LearningTaskNotFound(int id)
        {
            return $"Learning Task with Id '{id}' was not found.";
        }

        // Task Assignment
        public static string TaskAssignmentNotFound(int id)
        {
            return $"TaskAssignment with Id '{id}' was not found.";
        }

        // Submission
        public static string SubmissionNotFound(int id)
        {
            return $"Submission with Id '{id}' was not found.";
        }

        public static string SubmissionFileNotFound(int id)
        {
            return $"Submission file with Id '{id}' was not found.";
        }

        // Job Processing 
        public static string JobProcessingNotFound(int id)
        {
            return $"Job Processing with Id '{id}' was not found.";
        }

        // Enum
        public static string EnumFieldEmpty(string fieldName)
        {
            return $"The field '{fieldName}' cannot be null or empty.";
        }
        public static string EnumNotValid(string value, string fieldName, string validValues)
        {
            return $"Invalid value '{value}' for field '{fieldName}'. Valid options are: {validValues}";
        }


        // File Validation
        public const string EmptyFile = "The uploaded file cannot be empty.";

        public const string InvalidFileType = "The uploaded file type is not supported.";

        public const string FileTooLarge = "The uploaded file exceeds the maximum allowed size.";

        public const string InvalidJson = "The request payload format is invalid.";

        public const string UnexpectedError = "An unexpected error occurred. Please try again later.";

        public const string Unauthorized = "You are not authorized to perform this action.";
        public const string Forbidden = "You do not have permission to perform this action.";
        public const string FileNotFound = "File Not Found";
        public const string RabbitMQPublishFailed = "Submission saved but could not be queued for processing.";
    }
}

