using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using SendGrid;
using BGProcess.Interface;
using BGProcess.Models;

namespace BGProcess.Services
{
    public class EmailService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailQueue _emailQueue;
        private volatile bool _hasNewEmail;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IEmailQueue emailQueue)
        {
            _configuration = configuration;
            _logger = logger;
            _emailQueue = emailQueue;

            // Validate configuration at startup
            string apiKey = _configuration["SendGridAPIKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("SendGrid API key is not configured.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Service Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"EMAIL COUNT IN THE QUEUE: {_emailQueue.Queue.Count}");
                _logger.LogInformation($"{_hasNewEmail}");
                _logger.LogInformation($"THE QUEUE IS: {_emailQueue.Queue.Count}");
                if (_emailQueue.Queue.Count > 0)
                {
                    await ProcessEmails(stoppingToken);
                }
                else
                {
                    await Task.Delay(10000, stoppingToken); // Wait before checking again
                }
            }

            _logger.LogInformation("Email Service Stopped");
        }

        private async Task ProcessEmails(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Processing {_emailQueue.Queue.Count} emails");

            _logger.LogInformation("Start processing...");
            while (_emailQueue.Queue.TryDequeue(out var emailMessage))
            {
                try
                {
                    _logger.LogInformation($"Sending email to {emailMessage.ToEmail} with subject: {emailMessage.Subject}");
                    await SendEmail(emailMessage, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send email to {emailMessage.ToEmail} with subject: {emailMessage.Subject}");
                }
            }
        }

        private async Task SendEmail(EmailMessage emailMessage, CancellationToken stoppingToken)
        {
            var apiKey = _configuration["SendGridAPIKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("ars.hsrps@gmail.com", "ARS Company");
            var to = new EmailAddress(emailMessage.ToEmail);
            var plainTextContent = emailMessage.Message;
            var htmlContent = $"<strong>{System.Net.WebUtility.HtmlEncode(emailMessage.Message)}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, emailMessage.Subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg, stoppingToken);

            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogError($"Failed to send email. Status Code: {response.StatusCode}");
            }
            else
            {
                _logger.LogInformation($"Email sent to {emailMessage.ToEmail} with subject: {emailMessage.Subject}");
            }
        }
    }
}
