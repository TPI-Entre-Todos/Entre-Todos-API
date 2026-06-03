namespace Application.Interfaces;

public interface IEmailService
{
    Task EnviarEmailAsync(string to, string subject, string html);
}