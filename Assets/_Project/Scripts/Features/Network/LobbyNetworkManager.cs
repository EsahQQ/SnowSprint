using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Features.Network
{
    public class LobbyNetworkManager : NetworkBehaviour
    {
        public NetworkVariable<int> PlayersReadyCount { get; private set; } = new();
        public NetworkVariable<int> TotalPlayersCount { get; private set; } = new();
        
        public event Action<int, int> OnReadyStatsChanged;
        public event Action OnAllPlayersReady;
        
        private readonly HashSet<ulong> _readyClients = new();

        public override void OnNetworkSpawn()
        {
            PlayersReadyCount.OnValueChanged += HandleStatsChanged;
            TotalPlayersCount.OnValueChanged += HandleStatsChanged;

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
                
                TotalPlayersCount.Value = NetworkManager.Singleton.ConnectedClientsIds.Count;
            }
            
            HandleStatsChanged(0, 0);
        }

        public override void OnNetworkDespawn()
        {
            PlayersReadyCount.OnValueChanged -= HandleStatsChanged;
            TotalPlayersCount.OnValueChanged -= HandleStatsChanged;

            if (IsServer && NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerReadyServerRpc(ServerRpcParams rpcParams = default)
        {
            var clientId = rpcParams.Receive.SenderClientId;
            
            if (_readyClients.Add(clientId))
            {
                PlayersReadyCount.Value = _readyClients.Count;
                CheckStartGame();
            }
        }

        private void HandleClientConnected(ulong clientId)
        {
            TotalPlayersCount.Value = NetworkManager.Singleton.ConnectedClientsIds.Count;
            CheckStartGame();
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            _readyClients.Remove(clientId);
            PlayersReadyCount.Value = _readyClients.Count;
            TotalPlayersCount.Value = NetworkManager.Singleton.ConnectedClientsIds.Count;
            CheckStartGame();
        }

        private void HandleStatsChanged(int previousValue, int newValue) => OnReadyStatsChanged?.Invoke(PlayersReadyCount.Value, TotalPlayersCount.Value);

        private void CheckStartGame()
        {
            if (!IsServer || TotalPlayersCount.Value == 0) return;

            if (PlayersReadyCount.Value == TotalPlayersCount.Value)
                OnAllPlayersReady?.Invoke();
        }
    }
}