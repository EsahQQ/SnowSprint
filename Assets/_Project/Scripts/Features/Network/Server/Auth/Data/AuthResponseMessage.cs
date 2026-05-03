using Mirror;

namespace _Project.Scripts.Features.Network.Server.Auth.Data
{
    public struct AuthResponseMessage : NetworkMessage
    {
        public AuthRequestType Type;
        public bool Success;
        public string Message;
    }
}