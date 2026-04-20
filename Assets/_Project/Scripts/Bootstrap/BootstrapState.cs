using _Project.Scripts.Bootstrap.InitPipeline.Initializers.Global;
using _Project.Scripts.Bootstrap.InitPipeline.Initializers.Scene;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Bootstrap
{
    public class BootstrapState<TNextState> : BaseState  where TNextState : BaseState
    {
        private readonly IGlobalInitializer _globalInitializer;
        private readonly ISceneInitializer _sceneInitializer;

        public BootstrapState(IStateMachine stateMachine, IGlobalInitializer globalInitializer, ISceneInitializer sceneInitializer) : base(stateMachine)
        {
            _globalInitializer = globalInitializer;
            _sceneInitializer = sceneInitializer;
        }
        
        public override async UniTask OnEnter()
        {
            Debug.Log("BootstrapState Enter");
            
            if (!await _globalInitializer.EnsureInitializedAsync())
                Debug.Log("GlobalInit already done");
            if (!await _sceneInitializer.EnsureInitializedAsync())
                Debug.Log("SceneInit already done");
            
            StateMachine.RequestSwitchState<TNextState>();
        }
    }
}