using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

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
        public override void OnStartServer()
        {
            base.OnStartServer();
            UpdateTotalPlayers();
        }
        private void OnStatsHook(int oldValue, int newValue) => OnReadyStatsChanged?.Invoke(PlayersReadyCount, TotalPlayersCount);
        
        [ServerCallback]
        private void UpdateTotalPlayers()
        {
            int count = 0;
            foreach (var conn in NetworkServer.connections.Values)
                if (conn != null) count++;
            
            TotalPlayersCount = count;
        }
        
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
            /*if (TotalPlayersCount == 0) return;

            if (PlayersReadyCount == TotalPlayersCount)
                OnAllPlayersReady?.Invoke();*/
            
            if (TotalPlayersCount > 0 && PlayersReadyCount >= TotalPlayersCount)
            {
                Debug.Log("[Server] Все готовы! Запуск игры...");
                OnAllPlayersReady?.Invoke();
            }
        }
    }
}