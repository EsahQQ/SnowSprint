using System;
using System.Collections.Generic;
using Zenject;

namespace _Project.Scripts.Infrastructure.StateMachine.State
{
    public class FactoryStateZenject : IFactoryState
    {
        private readonly IInstantiator _container;
        private readonly Dictionary<Type, IState> _states = new();

        public FactoryStateZenject(IInstantiator container)
        {
            _container = container;
        }
        
        public IState GetState(Type stateType)
        {
            if (_states.TryGetValue(stateType, out var state))
            {
                state.Reset(); 
                return state;
            }

            var newState = _container.Instantiate(stateType) as IState;
            _states[stateType] = newState;
            return newState;
        }
    }
}