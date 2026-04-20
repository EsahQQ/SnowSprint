using System;

namespace _Project.Scripts.Infrastructure.StateMachine.State
{
    public interface IFactoryState
    {
        IState GetState(Type stateType);
    }
}
