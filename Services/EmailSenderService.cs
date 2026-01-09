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

            // Replace placeholders in the template dinamically
            foreach (var param in message.Parameters)
            {
                template.Subject = template.Subject.Replace(param.Key, param.Value);
                template.Body = template.Body.Replace(param.Key, param.Value);
            }

            var emailContent = new EmailContent(template.Subject)
            {
                PlainText = template.Body
            };

            var emailMessage = new EmailMessage(_settings.SenderEmail, message.Email, emailContent);

            try
            {
                //logging message details
                Console.WriteLine($"{DateTime.Now} - Sending email from: {_settings.SenderEmail} ");
                Console.WriteLine($"{DateTime.Now} - Sending email for RequestID: {message.RequestId} to {message.Email} using TemplateID: {message.TemplateId}");

                _emailClient.SendAsync(Azure.WaitUntil.Started, emailMessage);

                Console.WriteLine($"{DateTime.Now} - Email Request {message.RequestId} sent to Azure Communication Services.");
            }            
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - Failed to send email:");
                Console.WriteLine($"{DateTime.Now} - ex.Message: {ex.Message}");
                Console.WriteLine($"{DateTime.Now} - ex.StackTrace: {ex.StackTrace}");
                Console.WriteLine($"{DateTime.Now} - ex.InnerException: {ex.InnerException}");
                Console.WriteLine($"{DateTime.Now} - ex.HResult: {ex.HResult}");
                
                throw; // Re-throw the exception to handle it in the caller
            }
        }
    }
}