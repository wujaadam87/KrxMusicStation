using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace KrxMusicStation
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // pretend e-mail is sent
            return Task.Delay(1);
        }
    }
}
