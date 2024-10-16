using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGProcess.Models
{
    public class EmailMessage
    {
        public string ToEmail { get; }
        public string Subject { get; }
        public string Message { get; }

        public EmailMessage(string toEmail, string subject, string message)
        {
            this.ToEmail = toEmail;
            this.Subject = subject;
            this.Message = message;
        }
    }
}
