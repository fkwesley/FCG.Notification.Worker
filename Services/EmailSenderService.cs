using System;
using System.Threading.Tasks;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using FCG.Notification.Worker.Infrastructure.Configurations;
using FCG.Notification.Worker.Domain.Entities;

namespace FCG.Notification.Worker.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(EmailRequest message);
    }

    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _settings;
        private readonly EmailClient _emailClient;

        public EmailSenderService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
            _emailClient = new EmailClient(_settings.ConnectionString);
        }

        public async Task SendEmailAsync(EmailRequest message)
        {
            var template = EmailTemplate.GetTemplateById(message.TemplateId);

            // Replace placeholders in the template
            var subject = template.Subject.Replace("{orderId}", message.OrderId.ToString());
            var body = template.Body.Replace("{orderId}", message.OrderId.ToString());

            var emailContent = new EmailContent(subject)
            {
                PlainText = body
            };

            var emailMessage = new EmailMessage(_settings.SenderEmail, message.Email, emailContent);

            try
            {
                var response = await _emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage);
                Console.WriteLine($"Email sent successfully for OrderId: {message.OrderId}. MessageId: {response.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                throw; // Re-throw the exception to handle it in the caller
            }
        }
    }
}