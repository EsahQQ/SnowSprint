using _Project.Scripts.Features.Network;
using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Features.UI.Lobby;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates.Network
{
    public class LobbyState : BaseState
    {
        private readonly LobbyNetworkManager _lobbyNetworkManager;
        private readonly ILobbyView _lobbyView;
        
        public LobbyState(IStateMachine stateMachine, LobbyNetworkManager lobbyNetworkManager, ILobbyView lobbyView) 
            : base(stateMachine)
        {
            _lobbyNetworkManager = lobbyNetworkManager;
            _lobbyView = lobbyView;
        }

        public override async UniTask OnEnter()
        {
            Debug.Log("LobbyState Enter");
            
            _lobbyNetworkManager.OnAllPlayersReady += StartGame;
            
            await _lobbyView.ProcessLobbyAsync();
            
            if (_lobbyNetworkManager.IsSpawned)
            {
                _lobbyNetworkManager.SetPlayerReadyServerRpc(true);
            }
            else
            {
                Debug.LogError("[Network] Ошибка: Лобби Менеджер еще не заспавнен сетью!");
            }
        }

        public override UniTask OnExit()
        {
            _lobbyNetworkManager.OnAllPlayersReady -= StartGame;
            return UniTask.CompletedTask;
        }

        private void StartGame()
        {
            // Хост грузит сцену игры
            if (NetworkManager.Singleton.IsServer)
                StateMachine.RequestSwitchState<LoadSceneState, string>(SceneNames.Game);
        }
    }
}