using _Project.Scripts.Features.AppStates.Gameplay;
using _Project.Scripts.Features.Player.Factories;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates.SetupStates
{
    public class LevelSetupState : BaseState, IStateSetup
    {
        private readonly PlayerFactory _playerFactory;
        private readonly Transform _spawnPoint;
        private readonly Camera _camera;

        public LevelSetupState(IStateMachine stateMachine, PlayerFactory playerFactory, Transform spawnPoint, Camera camera) : base(stateMachine)
        {
            _playerFactory = playerFactory;
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
            var player = _playerFactory.Create(_spawnPoint);
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