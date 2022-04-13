namespace Homework.Models
{
    public interface IFile
    {
        string Filename { get; set; }
        byte[] Content { get; set; } 
    }
}
