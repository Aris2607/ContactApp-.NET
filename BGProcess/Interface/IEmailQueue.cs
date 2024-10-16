using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGProcess.Models;

namespace BGProcess.Interface
{
    public interface IEmailQueue
    {
        ConcurrentQueue<EmailMessage> Queue { get; }   
        Task EnqueueEmail(string toEmail, string subject, string message);
    }
}
