using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BusinessServices
{
    public class Utility
    {
   

        public static async Task<string> SendEmail(string toAddress, string subject, string body)
        {
            var client = new SendGridClient("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");
            var from = new EmailAddress("tomtran@carsaletest.com", "Tom Tran");

            var to = new EmailAddress(toAddress, "Carsale");
           
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", body);
            var response = await client.SendEmailAsync(msg);
            if (response.StatusCode == HttpStatusCode.Accepted)
                return "Ok";
            else
                return "Fail to send email. Error: " + response.ToString();
        }
    }
}
