namespace _Project.Scripts.Infrastructure.StateMachine.State
{
    public abstract class BaseFixedUpdateableState : BaseState, IFixedUpdateableState
    {
        protected BaseFixedUpdateableState(IStateMachine stateMachine) : base(stateMachine) { }

        public virtual void FixedUpdate(float fixedDt) { }
    }
}