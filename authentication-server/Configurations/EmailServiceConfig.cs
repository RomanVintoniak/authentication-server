namespace authentication_server.Configurations
{
    public class EmailServiceConfig
    {
        public int Port { get; set; }
        public string Host {  get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string AuthCode { get; set; } = string.Empty;
    }
}
