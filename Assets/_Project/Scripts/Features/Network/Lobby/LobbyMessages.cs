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
    
    public struct CreateRoomRequest : NetworkMessage { }

    public struct JoinRoomRequest : NetworkMessage 
    {
        public System.Guid RoomId; 
    }

    public struct GetRoomListRequest : NetworkMessage { }


// --- Сообщения от сервера к клиенту ---

    [System.Serializable]
    public struct RoomInfo
    {
        public System.Guid RoomId;
        public string RoomName; // Мы можем генерировать имена типа "Комната #123"
        public int PlayerCount;
        public int MaxPlayers;
    }

    public struct RoomListResponse : NetworkMessage
    {
        public RoomInfo[] Rooms;
    }

    public struct JoinRoomSuccess : NetworkMessage 
    {
        // Это сообщение говорит клиенту, что он успешно вошел и можно менять сцену
        public string RoomName;
    }

    public struct JoinRoomFailure : NetworkMessage 
    {
        public string Reason; // Например, "Комната заполнена" или "Комната не найдена"
    }
}