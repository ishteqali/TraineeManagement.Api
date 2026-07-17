namespace TraineeManagement.Api.Configurations
{
    public class FileStorageOptions
    {
        public const string SectionName = "FileStorage";
        public string RootPath { get; set; } = string.Empty;
        public int MaxFileSizeInMB { get; set; }
        public List<string> AllowedExtensions { get; set; } = [];
    }
}