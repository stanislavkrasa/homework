using Homework.Configurations;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace Homework.SmtpClients
{
    public class SmtpClientWrapper : ISmtpClient
    {
        private bool disposed;
        private readonly SmtpClient smtpClient;

        public SmtpClientWrapper(IOptions<SmtpClientWrapperOptions> options)
        {
            smtpClient = new SmtpClient(options.Value.Host, options.Value.Port);
        }

        ~SmtpClientWrapper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    smtpClient?.Dispose();
                }
                disposed = true;
            }
        }

        protected Task CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(SmtpClientWrapper));
            }

            return Task.CompletedTask;
        }

        public async Task Send(MailMessage mailMessage)
        {
            await CheckDisposed();
            smtpClient.Send(mailMessage);
        }
    }
}
