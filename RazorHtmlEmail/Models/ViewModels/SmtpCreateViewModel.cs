using System.ComponentModel.DataAnnotations;

namespace RazorHtmlEmail.Models.ViewModels
{
    public class SmtpCreateViewModel
    {
        [Required(ErrorMessage = "At least one recipient is required.")]
        public string Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
