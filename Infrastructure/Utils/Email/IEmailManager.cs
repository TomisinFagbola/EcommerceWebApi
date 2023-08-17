using Infrastructure.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utils.Email
{
    public interface IEmailManager
    {
        Task SendSingleMail(string receiverAddress, string message, string subject);
        //void SendBulkEmail(string[] receiverAddress, string message, string subject);
            void SendSingleEmailWithAttachment(string receiverAddress, string message, string subject, string fileName,
                string fileContent, string type);
            string GetConfirmEmailTemplate(string emailLink, User user);

             string GetSendOtpTemplate(string otp);
            string GetResetPasswordEmailTemplate(string emailLink, string email);

        
    }
}
