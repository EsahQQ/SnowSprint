using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates
{
    public class MainMenuState : BaseState
    {
        public MainMenuState(IStateMachine stateMachine) : base(stateMachine) { }
        
        public override UniTask OnEnter()
        {
            Debug.Log("MainMenuState Enter");
            return base.OnEnter();
        }
    }
}