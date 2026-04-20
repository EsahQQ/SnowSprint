namespace _Project.Scripts.Infrastructure.StateMachine.State.Payload
{
    public abstract class BasePayloadedState<TPayload> : BaseState, IPayloadedState<TPayload>, IPayloadInitializable
    {
        protected TPayload Payload { get; private set; }
        protected BasePayloadedState(IStateMachine stateMachine) : base(stateMachine) { }

        public void Initialize(TPayload payload) => Payload = payload;
        void IPayloadInitializable.SetPayload(object payload) => Initialize((TPayload)payload);
        public override void Reset() => Payload = default;
    }
}