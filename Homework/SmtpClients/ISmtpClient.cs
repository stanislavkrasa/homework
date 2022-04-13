using System.Net.Mail;

namespace Homework.SmtpClients
{
    public interface ISmtpClient : IDisposable
    {
        Task Send(MailMessage mailMessage);
    }
}
