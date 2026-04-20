namespace _Project.Scripts.Infrastructure.StateMachine.State.Payload
{
    public interface IPayloadedState<TPayload> : IState
    {
        void Initialize(TPayload payload);
    }
}