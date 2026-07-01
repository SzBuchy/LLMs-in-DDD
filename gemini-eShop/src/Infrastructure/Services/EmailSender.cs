using System.Threading.Tasks;
using Core.Interfaces;

namespace Infrastructure.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        // Mock email sender - does nothing in development
        return Task.CompletedTask;
    }
}
