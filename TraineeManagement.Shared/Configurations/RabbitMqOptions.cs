namespace TraineeManagement.Shared.Configurations
{
    public class RabbitMqOptions
    {
        public const string SectionName = "RabbitMQ";

        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string VirtualHost { get; set; } = "/";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
        public string DeadLetterExchange { get; set; } = string.Empty;
        public string DeadLetterQueue { get; set; } = string.Empty;
    }
}