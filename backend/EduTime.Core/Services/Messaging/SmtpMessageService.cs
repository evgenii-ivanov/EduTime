using System;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading;
using System.Threading.Tasks;
using EduTime.Core.Interfaces;
using EduTime.Dtos.Messaging;
using EduTime.Foundation.Options;

namespace EduTime.Core.Services.Messaging
{
    public class SmtpMessageService : IMessageService, IAsyncDisposable
    {
        private readonly SmtpConnectionOptions _smtpOptions;
        private readonly MessageSenderOptions _senderOptions;
        private readonly SmtpClient _smtpClient;

        public SmtpMessageService(IOptions<SmtpConnectionOptions> smtpOptions, IOptions<MessageSenderOptions> senderOptions)
        {
            _smtpOptions = smtpOptions.Value;
            _senderOptions = senderOptions.Value;
            _smtpClient = new SmtpClient();
        }

        public async Task<bool> SendAsync(MessageDto model, CancellationToken cancellationToken)
        {
            var emailMessage = new MimeMessage();
            emailMessage.Headers.Add(new Header(HeaderId.ListUnsubscribe, "remove reference"));// TODO change "remove reference" to something actual
            emailMessage.From.Add(new MailboxAddress(_senderOptions.Name, _senderOptions.Address));
            emailMessage.To.Add(new MailboxAddress(model.Name, model.Recipient));
            emailMessage.Subject = model.Subject;

            var builder = new BodyBuilder {HtmlBody = model.PlainText};
            emailMessage.Body = builder.ToMessageBody();

            if (!_smtpClient.IsConnected)
            {
                if (_smtpOptions.AllowUntrustedSsl)
                    // This option should be enabled for test envs only. Allows insecure connection.
                    _smtpClient.ServerCertificateValidationCallback = (_, _, _, _) => true;

                await _smtpClient.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, _smtpOptions.Ssl,
                    cancellationToken);
            }

            if (!_smtpClient.IsAuthenticated && _smtpOptions.SupportsAuthentication)
            {
                await _smtpClient.AuthenticateAsync(_smtpOptions.Login, _smtpOptions.Password,
                    cancellationToken);
            }

            await _smtpClient.SendAsync(emailMessage, cancellationToken);

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_smtpClient.IsConnected)
                await _smtpClient.DisconnectAsync(true, CancellationToken.None);
        }
    }
}
