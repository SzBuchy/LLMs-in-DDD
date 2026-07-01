using System.Threading.Tasks;

namespace Core.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}
