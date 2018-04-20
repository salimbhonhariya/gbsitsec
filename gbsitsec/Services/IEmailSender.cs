using System.Threading.Tasks;

namespace gbsitsec.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}