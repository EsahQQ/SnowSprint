using System.Collections.Generic;
using Mirror;

namespace _Project.Scripts.Features.Network.Lobby
{
    public class MatchRoom
    {
        public System.Guid RoomId { get; }
        public List<NetworkConnectionToClient> Players { get; } = new List<NetworkConnectionToClient>();
        private readonly HashSet<int> _readyPlayers = new HashSet<int>();
        private readonly int _maxPlayers;

        public string RoomName { get; }
        private static int _roomCounter = 1;
        
        public bool IsFull => Players.Count >= _maxPlayers;
        public int ReadyCount => _readyPlayers.Count;

        public MatchRoom(int maxPlayers)
        {
            RoomId = System.Guid.NewGuid();
            _maxPlayers = maxPlayers;
            RoomName = $"Комната #{_roomCounter++}";
        }

        public void AddPlayer(NetworkConnectionToClient conn)
        {
            if (!IsFull)
                Players.Add(conn);
        }

        public void RemovePlayer(NetworkConnectionToClient conn)
        {
            Players.Remove(conn);
            _readyPlayers.Remove(conn.connectionId);
        }

        public void SetPlayerReady(NetworkConnectionToClient conn)
        {
            _readyPlayers.Add(conn.connectionId);
        }

        public bool AreAllPlayersReady()
        {
            return _readyPlayers.Count == Players.Count;
        }
        
        public void ResetReadyState()
        {
            _readyPlayers.Clear();
        }
    }
}