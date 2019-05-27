namespace RazorHtmlEmail.Models.Settings
{
    public class SmtpSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpUseSsl { get; set; }
        public string SmtpAccount { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpFromName { get; set; }
        public string SmtpFromAddress { get; set; }
    }
}
