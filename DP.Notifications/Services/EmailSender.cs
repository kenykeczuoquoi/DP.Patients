using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DP.Notifications.Services
{
    public class EmailSender
    {

        public void SendNewUserEmail(string email)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("dp102s9844sw@gmail.com", "2wsx!QAZ"),
                EnableSsl = true,
            };

            smtpClient.Send("dp102s9844sw@gmail.com", email, "Covid19", "Wiadomość o kwarantannie");
        }
    }
}
