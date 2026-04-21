using _Project.Scripts.Features.AppStates.Gameplay;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using _Project.Scripts.Libs.Factories;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates.SetupStates
{
    public class LevelSetupState : BaseState, IStateSetup
    {
        private readonly IFactory<PlayerController> _factoryPlayer;
        private readonly IPlayerProvider _playerProvider;
        private readonly Transform _spawnPoint;
        private readonly Camera _camera;

        public LevelSetupState(IStateMachine stateMachine, IFactory<PlayerController> factoryPlayer,
            IPlayerProvider playerProvider, Transform spawnPoint ,Camera camera) : base(stateMachine)
        {
            _factoryPlayer = factoryPlayer;
            _playerProvider = playerProvider;
            _spawnPoint = spawnPoint;
            _camera = camera;
        }

        public override async UniTask OnEnter()
        {
            Debug.Log("LevelSetupState Enter");
            await Setup();
            
            StateMachine.RequestSwitchState<GameplayLoopState>();
        }

        public async UniTask Setup()
        {
            var player = _factoryPlayer.Create();
            player.transform.position = _spawnPoint.position;
            _playerProvider.RegisterPlayer(player);
            if (_camera.TryGetComponent<CinemachineBrain>(out var brain))
            {
                var vcam = brain.ActiveVirtualCamera as CinemachineCamera;
                if (vcam != null)
                    vcam.Follow = player.transform;
            }
            
            await UniTask.CompletedTask;
        }
    }
}