using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Homework.Models.ApiDTO.Requests
{
    public class ConvertByApiPayload
    {
        [Required]
        public string DocumentType { get; set; } = String.Empty;

        [Required]
        public string Base64Content { get; set; } = String.Empty;

        [Required]
        public FileFormat InputFormat { get; set; }

        [Required]
        public FileFormat OutputFormat { get; set; }
    }
}
