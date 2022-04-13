namespace Homework.Configurations
{
    public class SmtpClientWrapperOptions
    {
        public const string ConfigurationSectionName = "SmtpClientWrapper";

        public string Host { get; set; } = String.Empty;
        public int Port { get; set; } = 25;
    }
}
