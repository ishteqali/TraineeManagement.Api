namespace TraineeManagement.Api.Configurations
{
    public class DatabaseOptions
    {
        public const string SectionName = "DbSettings";

        public string Host { get; set; } = string.Empty;

        public string Port { get; set; } = string.Empty;

        public string Database { get; set; } = string.Empty;

        public string User { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

