using Hangfire;
using Infrastructure.Entities.Identity;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utils.Email
{
    public class EmailManager : IEmailManager
    {
        private readonly SendGridClient _clientKey;
        private readonly IConfiguration _config;
        private readonly EmailAddress _from;

        public EmailManager(IConfiguration configuration)
        {
            _config = configuration;
            var sendGridKey = configuration["SENDGRID_KEY"] ?? "SENDGRID_KEY";
            var senderEmail = configuration["SENDER_EMAIL"] ?? "SENDER_EMAIL";
            _clientKey = new SendGridClient(sendGridKey);
            _from = new EmailAddress(senderEmail);
        }


        //public void SendBulkEmail(string[] receiverAddress, string message, string subject)
        //{
        //    BackgroundJob.Enqueue(() => SendBulkMail(receiverAddress, message, subject));
        //}

        //public void SendSingleEmail(string receiverAddress, string message, string subject)
        //{
        //    BackgroundJob.Enqueue(() => SendSingleMail(receiverAddress, message, subject));
        //}

        public void SendSingleEmailWithAttachment(string receiverAddress, string message, string subject,
            string fileName, string fileContent, string type)
        {
            BackgroundJob.Enqueue(() =>
                SendSingleMailWithAttachment(receiverAddress, message, subject, fileName, fileContent, type));
        }

        public async Task SendSingleMail(string receiverAddress, string message, string subject)
        {
            var To = new EmailAddress(receiverAddress);
            var plainText = message;
            var htmlContent = message;
            var msg = MailHelper.CreateSingleEmail(_from, To, subject, plainText, htmlContent);
            var response = await _clientKey.SendEmailAsync(msg);

            // Throw an exception if the response is not successful, so that hangfire can retry
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

        }

        public async Task SendBulkMail(string[] receiverAddress, string message, string subject)
        {
            var Tos = new List<EmailAddress>();

            foreach (var item in receiverAddress)
                Tos.Add(new EmailAddress(item));


            var plainText = "";
            var htmlContent = @$"
                <html><body><p>{message}</p></body></html>
            ";

            var msg = MailHelper
                .CreateSingleEmailToMultipleRecipients(_from, Tos, subject, plainText, htmlContent);
            var response = await _clientKey.SendEmailAsync(msg);

            //Throw an exception if the response is not successful, so that hangfire can retry
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

        }

        /// <summary>
        /// Access modifier should be public for Background service
        /// <see cref="https://stackoverflow.com/questions/54437221/how-to-resolve-only-public-methods-can-be-invoked-in-the-background-in-hangfire"/>
        /// </summary>
        public async Task SendSingleMailWithAttachment(string receiverAddress, string message, string subject,
            string fileName, string fileContent, string type = "application/pdf")
        {
            var To = new EmailAddress(receiverAddress);
            var plainText = message;
            var htmlContent = message;
            var msg = MailHelper.CreateSingleEmail(_from, To, subject, plainText, htmlContent);


            var attachment = new SendGrid.Helpers.Mail.Attachment
            {
                Content = fileContent,
                ContentId = Guid.NewGuid().ToString(),
                Disposition = "inline",
                Filename = fileName,
                Type = type

            };

            msg.AddAttachment(attachment);
            var response = await _clientKey.SendEmailAsync(msg);

            //Throw an exception if the response is not successful, so that hangfire can retry
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());
        }

        public string GetConfirmEmailTemplate(string emailLink, User user)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "ConfirmEmail.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{{action_url}}", emailLink)
                .Replace("{{org_name}}", "EZ Cash 9-5")
                .Replace("{{_name}}", $"{user.FirstName} {user.LastName}")
                .Replace("{email}", user.Email);

            return msgBody;
        }
        public string GetSendOtpTemplate(string otp)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "SendOtp.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{otp}", otp);

            return msgBody;
        }

        public string GetResetPasswordEmailTemplate(string emailLink, string email)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "ResetPassword.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{email_link}", emailLink).
                Replace("{email}", email);

            return msgBody;
        }

    }
}
