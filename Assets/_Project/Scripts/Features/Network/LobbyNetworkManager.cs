using System;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Features.Network
{
    public class LobbyNetworkManager : NetworkBehaviour
    {
        public NetworkVariable<int> PlayersReadyCount { get; private set; } = new();
        public event Action OnAllPlayersReady;

        private void Start() => PlayersReadyCount.OnValueChanged += HandlePlayersReadyChanged;

        public override void OnDestroy()
        {
            PlayersReadyCount.OnValueChanged -= HandlePlayersReadyChanged;
            base.OnDestroy();
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerReadyServerRpc(bool isReady)
        {
            if (isReady)
                PlayersReadyCount.Value++;
            else
                PlayersReadyCount.Value--;
        }

        private void HandlePlayersReadyChanged(int previousValue, int newValue)
        {
            Debug.Log($"[Network] Готово игроков: {newValue} / {NetworkManager.Singleton.ConnectedClientsIds.Count}");
            
            if (IsServer && newValue == NetworkManager.Singleton.ConnectedClientsIds.Count && newValue > 0)
                OnAllPlayersReady?.Invoke();
        }
    }
}