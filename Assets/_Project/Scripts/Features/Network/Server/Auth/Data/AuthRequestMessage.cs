using Mirror;

namespace _Project.Scripts.Features.Network.Server.Auth.Data
{
    public struct AuthRequestMessage : NetworkMessage
    {
        public AuthRequestType Type;
        public string Email;
        public string Password;
        public string Username;
        public string Code;
    }
}