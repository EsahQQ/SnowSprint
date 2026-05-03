using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Bootstrap
{
    public class BootstrapState<TNextState> : BaseState  where TNextState : BaseState
    {
        public BootstrapState(IStateMachine stateMachine) : base(stateMachine) { }
        
        public override UniTask OnEnter()
        {
            StateMachine.RequestSwitchState<TNextState>();
            return UniTask.CompletedTask;
        }
    }
}