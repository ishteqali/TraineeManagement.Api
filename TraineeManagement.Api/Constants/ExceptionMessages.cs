namespace TraineeManagement.Api.Constants
{
    public static class ExceptionMessages
    {
        // Trainee
        public static string TrianeeNotFound(int id)
        {
            return $"Trianee with Id '{id}' was not found.";
        }

        // Trainee
        public static string MentorNotFound(int id)
        {
            return $"Mento with Id '{id}' was not found.";
        }

        // Trainee
        public static string LearningTaskNotFound(int id)
        {
            return $"Learning Task with Id '{id}' was not found.";
        }

        // Trainee
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

        // File Validation
        public const string EmptyFile = "The uploaded file cannot be empty.";

        public const string InvalidFileType = "The uploaded file type is not supported.";

        public const string FileTooLarge = "The uploaded file exceeds the maximum allowed size.";

        public const string InvalidJson = "The request payload format is invalid.";

        public const string UnexpectedError = "An unexpected error occurred. Please try again later.";

        public const string Unauthorized = "You are not authorized to perform this action.";
        public const string Forbidden = "You do not have permission to perform this action.";
    }
}

