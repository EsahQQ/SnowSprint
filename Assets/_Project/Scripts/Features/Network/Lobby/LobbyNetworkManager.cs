using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace _Project.Scripts.Features.Network.Lobby
{
    public class LobbyNetworkManager : NetworkBehaviour
    {
        public static LobbyNetworkManager Singleton { get; private set; }

        public override void OnStartServer()
        {
            Singleton = this;
        }

        private void Awake()
        {
            Debug.Log($"[LNM] Awake. isServer={isServer} isClient={isClient}");
        }

        public override void OnStartClient()
        {
            Debug.Log($"[LNM] OnStartClient. netId={netId}");
            Singleton = this;
        }

        public override void OnStopClient()
        {
            if (Singleton == this) Singleton = null;
        }

        public override void OnStopServer()
        {
            if (Singleton == this) Singleton = null;
        }

        [SyncVar(hook = nameof(OnStatsChanged))]
        public int PlayersReadyCount;

        [SyncVar(hook = nameof(OnStatsChanged))]
        public int TotalPlayersCount;

        public event Action<int, int> OnReadyStatsChanged;
        public event Action OnAllPlayersReady;

        private readonly HashSet<int> _readyClients = new();

        [Command(requiresAuthority = false)]
        public void CmdSetPlayerReady(NetworkConnectionToClient sender = null)
        {
            if (sender == null) return;

            if (_readyClients.Add(sender.connectionId))
            {
                PlayersReadyCount = _readyClients.Count;
                UpdateTotalPlayers();
            }
        }

        [ServerCallback]
        private void Update()
        {
            if (NetworkServer.connections.Count != TotalPlayersCount)
                UpdateTotalPlayers();
        }

        [ServerCallback]
        private void UpdateTotalPlayers()
        {
            TotalPlayersCount = NetworkServer.connections.Count;
            CheckAllReady();
        }

        private void OnStatsChanged(int _, int __)
        {
            Debug.Log($"[Lobby] SyncVar → {PlayersReadyCount}/{TotalPlayersCount}");
            OnReadyStatsChanged?.Invoke(PlayersReadyCount, TotalPlayersCount);
        }

        [Server]
        private void CheckAllReady()
        {
            if (TotalPlayersCount > 0 && PlayersReadyCount >= TotalPlayersCount)
            {
                Debug.Log("[Lobby] Все готовы! Уведомляем подписчиков...");
                OnAllPlayersReady?.Invoke();
            }
        }
    }
}
