using System;

namespace _Project.Scripts.Infrastructure.StateMachine.Transition
{
    public class RequestSwitchState
    {
        public Type StateType { get; }
        public object Payload { get; }

        public RequestSwitchState(Type stateType, object payload = null)
        {
            StateType = stateType ?? throw new ArgumentNullException(nameof(stateType));
            Payload = payload;
        }
    }
}
