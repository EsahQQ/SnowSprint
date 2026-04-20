using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Gameplay.Player;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates.Gameplay
{
    public class GameplayLoopState : BaseState
    {
        private readonly PlayerController _player;
        private readonly FinishTrigger _finishTrigger;
        private readonly IHudView _hudView;

        public GameplayLoopState(IStateMachine stateMachine, PlayerController player, FinishTrigger finishTrigger, IHudView hudView) 
            : base(stateMachine)
        {
            _player = player;
            _finishTrigger = finishTrigger;
            _hudView = hudView;
        }

        public override UniTask OnEnter()
        {
            Debug.Log("GameplayLoopState Enter");
            
            _hudView.Show();
            _player.SetActive(true);
            _finishTrigger.OnPlayerFinished += OnLevelFinished;
            
            return UniTask.CompletedTask;
        }

        public override UniTask OnExit()
        {
            _finishTrigger.OnPlayerFinished -= OnLevelFinished;
            
            _hudView.Hide();
            _player.SetActive(false);
            
            return UniTask.CompletedTask;
        }

        private void OnLevelFinished()
        {
            StateMachine.RequestSwitchState<LevelFinishedState>();
        }
    }
}