using System.ComponentModel.DataAnnotations;

namespace Homework.Models.ApiDTO.Requests
{
    public class ConvertOnStoragePayload
    {
        [Required]
        public string DocumentType { get; set; } = String.Empty;

        [Required]
        public string InputFilename { get; set; } = String.Empty;

        [Required]
        public FileFormat InputFormat { get; set; }

        public string InputFileLocation { get; set; } = String.Empty;

        [Required]
        public string OutputFilename { get; set; } = String.Empty;

        [Required]
        public FileFormat OutputFormat { get; set; }

        public string OutputFileLocation { get; set; } = String.Empty;
    }
}
