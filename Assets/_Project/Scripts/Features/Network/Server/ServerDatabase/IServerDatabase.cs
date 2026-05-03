using _Project.Scripts.Features.Network.Server.ServerDatabase.Data;

namespace _Project.Scripts.Features.Network.Server.ServerDatabase
{
    public interface IServerDatabase
    {
        bool IsEmailExists(string email);
        string CreateUserAndGetCode(string email, string username, string password);
        bool TryVerify(string email, string code);
        UserData TryLogin(string email, string password);
    }
}