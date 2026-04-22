using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Features.UI.HUD;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates.Gameplay
{
    public class GameplayLoopState : BaseFixedUpdateableState
    {
        private readonly IPlayerProvider _playerProvider;
        private readonly LevelProgressView _levelProgressView;
        private readonly FinishTrigger _finishTrigger;
        private readonly IHudView _hudView;

        public GameplayLoopState(IStateMachine stateMachine, IPlayerProvider playerProvider, FinishTrigger finishTrigger, IHudView hudView, LevelProgressView levelProgressView) : base(stateMachine)
        {
            _playerProvider = playerProvider;
            _finishTrigger = finishTrigger;
            _hudView = hudView;
            _levelProgressView = levelProgressView;
        }

        public override UniTask OnEnter()
        {
            Debug.Log("GameplayLoopState Enter");
            
            _hudView.Show();
            _playerProvider.Player.Initialize();
            _playerProvider.Player.SetActive(true);
            _finishTrigger.OnPlayerFinished += OnLevelFinished;

            _levelProgressView.Init();
            
            return UniTask.CompletedTask;
        }

        public override UniTask OnExit()
        {
            _finishTrigger.OnPlayerFinished -= OnLevelFinished;
            _playerProvider.Player.SetActive(false);
            _hudView.Hide();
            
            return UniTask.CompletedTask;
        }

        private void OnLevelFinished() => StateMachine.RequestSwitchState<LevelFinishedState>();

        public override void Update(float dt)
        {
            _playerProvider.Player.Tick();
            _levelProgressView.Update();
        }

        public override void FixedUpdate(float fixedDt)
        {
            _playerProvider.Player.FixedTick(fixedDt);
        }
    }
}