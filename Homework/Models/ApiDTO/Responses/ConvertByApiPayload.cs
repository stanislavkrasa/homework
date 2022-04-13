using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Homework.Models.ApiDTO.Responses
{
    public class ConvertByApiPayload
    {
        [Required]
        public string Base64Content { get; set; } = String.Empty;

        [Required]
        public FileFormat OuputFormat { get; set; }
    }
}
