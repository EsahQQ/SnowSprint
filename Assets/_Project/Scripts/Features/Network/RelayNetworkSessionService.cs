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
            await InitializeUnityServicesAsync();

            Debug.Log("[Network] Ищем свободное лобби...");
            
            bool joined = await TryQuickJoinLobbyAsync();
            
            if (!joined)
            {
                Debug.Log("[Network] Свободных лобби нет. Создаем свое...");
                await CreateLobbyAndRelayAsync();
            }
        }

        private async UniTask InitializeUnityServicesAsync()
        {
            if (_isInitialized) return;
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"[Network] Авторизован: {AuthenticationService.Instance.PlayerId}");
            }
            _isInitialized = true;
        }

        private async UniTask<bool> TryQuickJoinLobbyAsync()
        {
            try
            {
                var lobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                string joinCode = lobby.Data[RelayCodeKey].Value;
                Debug.Log($"[Network] Лобби найдено! Код Relay: {joinCode}. Подключаемся...");

                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetRelayServerData(new Unity.Networking.Transport.Relay.RelayServerData(joinAllocation, "dtls"));

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
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                
                var lobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Data = new Dictionary<string, DataObject>
                    {
                        { RelayCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                    }
                };
                
                await LobbyService.Instance.CreateLobbyAsync("SkiRaceLobby", MaxPlayers, lobbyOptions);
                Debug.Log($"[Network] Лобби создано. Relay код: {joinCode}");
                
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetRelayServerData(new Unity.Networking.Transport.Relay.RelayServerData(allocation, "dtls"));

                StartHost();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Network] Ошибка создания игры: {e.Message}");
            }
        }

        public void StartHost() => NetworkManager.Singleton.StartHost();
        public void StartClient() => NetworkManager.Singleton.StartClient();

        public void SetLocalPlayerReady(bool isReady)
        {
            if (IsHost) 
            {
                Debug.Log("[Network] Временно: Хост готов -> Запускаем игру");
                OnAllPlayersReady?.Invoke();
            }
        }
    }
}