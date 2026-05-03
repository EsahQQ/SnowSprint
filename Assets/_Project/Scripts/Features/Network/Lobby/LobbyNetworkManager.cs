using System;
using System.Collections.Generic;
using Mirror;

namespace _Project.Scripts.Features.Network.Lobby
{
    public class LobbyNetworkManager : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnStatsHook))]
        public int PlayersReadyCount;

        [SyncVar(hook = nameof(OnStatsHook))]
        public int TotalPlayersCount;

        public event Action<int, int> OnReadyStatsChanged;
        public event Action OnAllPlayersReady;
        
        private readonly HashSet<NetworkConnection> _readyClients = new();

        public override void OnStartClient() => OnReadyStatsChanged?.Invoke(PlayersReadyCount, TotalPlayersCount);
        public override void OnStartServer() => TotalPlayersCount = NetworkServer.connections.Count;
        private void OnStatsHook(int oldValue, int newValue) => OnReadyStatsChanged?.Invoke(PlayersReadyCount, TotalPlayersCount);
        
        [Command(requiresAuthority = false)]
        public void CmdSetPlayerReady(NetworkConnectionToClient sender = null)
        {
            if (sender == null) return;

            if (_readyClients.Add(sender))
            {
                PlayersReadyCount = _readyClients.Count;
                TotalPlayersCount = NetworkServer.connections.Count;
                CheckStartGame();
            }
        }

        [ServerCallback]
        private void Update()
        {
            if (_readyClients.RemoveWhere(conn => conn == null || !conn.isReady) > 0)
                PlayersReadyCount = _readyClients.Count;
            
            if (TotalPlayersCount != NetworkServer.connections.Count)
            {
                TotalPlayersCount = NetworkServer.connections.Count;
                CheckStartGame();
            }
        }

        [Server]
        private void CheckStartGame()
        {
            if (TotalPlayersCount == 0) return;

            if (PlayersReadyCount == TotalPlayersCount)
                OnAllPlayersReady?.Invoke();
        }
    }
}