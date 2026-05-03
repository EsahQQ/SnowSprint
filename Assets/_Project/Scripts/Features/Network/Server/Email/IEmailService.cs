namespace _Project.Scripts.Features.Network.Server.Email
{
    public interface IEmailService
    {
        void SendVerificationEmail(string toEmail, string code);
    }
}