using System;
using _Project.Scripts.Infrastructure.StateMachine.State;
using _Project.Scripts.Infrastructure.StateMachine.State.Payload;
using _Project.Scripts.Infrastructure.StateMachine.Transition;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.StateMachine
{
    public class StateMachine : IStateMachine
    {
        private readonly IFactoryState _stateFactory;
        private IState _currentState;
        private bool _isTransitioning;
        private RequestSwitchState _pendingRequest;

        public IState CurrentState => _currentState;

        public StateMachine(IFactoryState stateFactory)
        {
            _stateFactory = stateFactory ?? throw new ArgumentNullException(nameof(stateFactory));
        }

        public void RequestSwitchState<T>() where T : IState
        {
            if (_pendingRequest is not null)
                throw new InvalidOperationException($"State switch already requested to {_pendingRequest.StateType.Name}. Process the current request in Update before requesting another.");

            _pendingRequest = new RequestSwitchState(typeof(T));
        }
        
        public void RequestSwitchState(Type t)
        {
            if (_pendingRequest is not null)
                throw new InvalidOperationException($"State switch already requested to {_pendingRequest.StateType.Name}. Process the current request in Update before requesting another.");

            _pendingRequest = new RequestSwitchState(t);
        }
        
        public void RequestSwitchState<TState, TPayload>(TPayload payload) where TState : IPayloadedState<TPayload>
        {
            if (_pendingRequest is not null)
                throw new InvalidOperationException($"State switch already requested to {_pendingRequest.StateType.Name}. Process the current request in Update before requesting another.");

            _pendingRequest = new RequestSwitchState(typeof(TState), payload);
        }

        public void Update(float dt)
        {
            if (_isTransitioning) 
                return;
            
            _currentState?.Update(dt);

            if (_pendingRequest is null || _isTransitioning)
                return;

            var request = _pendingRequest;
            _pendingRequest = null;
            TransitionToAsync(request).Forget();
        }
        
        public void FixedUpdate(float fixedDt)
        {
            if (_isTransitioning) return;
            
            if (_currentState is IFixedUpdateableState fixedUpdateable)
                fixedUpdateable.FixedUpdate(fixedDt);
        }

        private async UniTaskVoid TransitionToAsync(RequestSwitchState request)
        {
            _isTransitioning = true;

            try
            {
                if (_currentState != null)
                    await _currentState.OnExit();

                var newState = _stateFactory.GetState(request.StateType);
                if (newState is null)
                    throw new InvalidOperationException($"State factory returned null for type {request.StateType.Name}.");
                
                if (request.Payload != null && newState is IPayloadInitializable initializable)
                    initializable.SetPayload(request.Payload);
                
                _currentState = newState;
                await _currentState.OnEnter();
            }
            finally
            {
                _isTransitioning = false;
            }
        }
    }
}