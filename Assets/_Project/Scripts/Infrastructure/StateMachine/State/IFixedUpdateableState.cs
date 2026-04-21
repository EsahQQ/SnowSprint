namespace _Project.Scripts.Infrastructure.StateMachine.State
{
    public interface IFixedUpdateableState
    {
        void FixedUpdate(float fixedDt);
    }
}