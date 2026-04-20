using System;
using _Project.Scripts.Infrastructure.StateMachine.State;
using _Project.Scripts.Infrastructure.StateMachine.State.Payload;

namespace _Project.Scripts.Infrastructure.StateMachine
{
    public interface IStateMachine 
    {
        void RequestSwitchState<T>() where T :  IState;
        void RequestSwitchState(Type t);
        void RequestSwitchState<TState, TPayload>(TPayload payload) where TState : IPayloadedState<TPayload>;
        void Update(float deltaTime);
    }
}