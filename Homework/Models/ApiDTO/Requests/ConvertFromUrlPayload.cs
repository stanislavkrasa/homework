using System.ComponentModel.DataAnnotations;

namespace Homework.Models.ApiDTO.Requests
{
    public class ConvertFromUrlPayload
    {
        [Required]
        public string DocumentType { get; set; } = String.Empty;

        [Required]
        public string Url { get; set; } = String.Empty;

        [Required]
        public FileFormat InputFormat { get; set; }

        [Required]
        public FileFormat OutputFormat { get; set; }
    }
}
