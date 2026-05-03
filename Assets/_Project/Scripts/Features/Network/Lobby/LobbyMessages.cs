using Mirror;

namespace _Project.Scripts.Features.Network.Lobby
{
    public struct LobbyStatusMessage : NetworkMessage
    {
        public int ReadyCount;
        public int TotalCount;
    }

    public struct LobbyReadyMessage : NetworkMessage { }
    public struct LobbyStartGameMessage : NetworkMessage { }
}