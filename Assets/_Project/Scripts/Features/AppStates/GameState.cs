using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Features.AppStates
{
    public class GameState : BaseState
    {
        public GameState(IStateMachine stateMachine) : base(stateMachine) { }
        
        public override UniTask OnEnter()
        {
            Debug.Log("LevelGameState Enter");
            return base.OnEnter();
        }
    }
}