using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.EntryPoint
{
    public class SceneEntryPoint<TStartState> : IInitializable, ITickable where TStartState : BaseState
    {
        private readonly IStateMachine _stateMachine;

        private SceneEntryPoint(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Initialize()
        {
            _stateMachine.RequestSwitchState<BootstrapState<TStartState>>();
        }

        public void Tick()
        {
            _stateMachine.Update(Time.deltaTime);
        }
    }
}