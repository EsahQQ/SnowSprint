using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

namespace _Project.Scripts.Features.Network
{
    public class RelayNetworkSessionService : INetworkSessionService
    {
        public bool IsHost => NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost;
        public bool IsConnected => NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient;
        public event Action OnAllPlayersReady;

        private const int MaxPlayers = 2; 
        private const string RelayCodeKey = "RelayJoinCode"; 
        private bool _isInitialized;

        public async UniTask QuickJoinOrCreateAsync()
        {
            if (!await TryQuickJoinLobbyAsync()) 
                await CreateLobbyAndRelayAsync();
        }

        private async UniTask<bool> TryQuickJoinLobbyAsync()
        {
            try
            {
                var lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobby.Data[RelayCodeKey].Value);
                NetworkManager.Singleton
                    .GetComponent<UnityTransport>()
                    .SetRelayServerData(new Unity.Networking.Transport.Relay.RelayServerData(joinAllocation, "dtls"));
                StartClient();
                return true;
            }
            catch (LobbyServiceException)
            {
                return false;
            }
        }

        private async UniTask CreateLobbyAndRelayAsync()
        {
            try
            {
                var allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                await LobbyService.Instance.CreateLobbyAsync("SkiRaceLobby", MaxPlayers, new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Data = new Dictionary<string, DataObject>
                        { { RelayCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
                });

                NetworkManager.Singleton.GetComponent<UnityTransport>()
                    .SetRelayServerData(new Unity.Networking.Transport.Relay.RelayServerData(allocation, "dtls"));
                StartHost();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Network] Ошибка: {e.Message}");
            }
        }

        public void StartHost() => NetworkManager.Singleton.StartHost();
        public void StartClient() => NetworkManager.Singleton.StartClient();
        public void SetLocalPlayerReady(bool isReady) { } 
    }
}