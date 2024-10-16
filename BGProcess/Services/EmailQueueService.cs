using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGProcess.Interface;
using BGProcess.Models;

namespace BGProcess.Services
{
    public class EmailQueueService : IEmailQueue
    {
        public ConcurrentQueue<EmailMessage> Queue { get; private set; } = new ConcurrentQueue<EmailMessage>();

        public async Task EnqueueEmail(string toEmail, string subject, string message)
        {
            EmailMessage emailMessage = new EmailMessage(toEmail, subject, message);
            Queue.Enqueue(emailMessage);
        }
    }
}
