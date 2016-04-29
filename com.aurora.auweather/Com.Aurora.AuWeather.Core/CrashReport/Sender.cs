using Com.Aurora.Shared;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Com.Aurora.AuWeather.Core.CrashReport
{
    public static class Sender
    {
        public static async Task SendFileEmail(StorageFile attachmentFile, string messageBody = "\n")
        {
            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
            emailMessage.Body = messageBody;

            if (attachmentFile != null)
            {
                var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(attachmentFile);

                var attachment = new Windows.ApplicationModel.Email.EmailAttachment(
                    attachmentFile.Name,
                    stream);

                emailMessage.Attachments.Add(attachment);
            }
            var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient(Utils.MAIL_ADDRESS);
            emailMessage.To.Add(emailRecipient);

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);

        }

    }
}
