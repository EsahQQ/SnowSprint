using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.StateMachine.State
{
    public abstract class BaseState : IState
    {
        protected IStateMachine StateMachine { get; private set; }
        
        protected BaseState(IStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        
        public virtual UniTask OnEnter() => UniTask.CompletedTask;
        public virtual UniTask OnExit() => UniTask.CompletedTask;
        public virtual void Update(float dt) { }
        public virtual void Reset() { }
    }
}
