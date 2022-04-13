namespace Homework.Models
{
    public class File : IFile
    {
        public string Filename { get; set; }
        public byte[] Content { get; set; }

        public File(string filename, byte[] content)
        {
            Filename = filename;
            Content = content;
        }
    }
}
