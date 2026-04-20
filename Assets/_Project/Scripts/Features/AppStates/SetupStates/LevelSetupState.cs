using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates.SetupStates
{
    public class LevelSetupState : BaseState, IStateSetup
    {
        public LevelSetupState(IStateMachine stateMachine) : base(stateMachine) { }

        public override async UniTask OnEnter()
        {
            Debug.Log("LevelSetup State...");
            await Setup();
            StateMachine.RequestSwitchState<GameState>();
        }

        public async UniTask Setup()
        {
            await UniTask.Delay(1000);
        }
    }
}