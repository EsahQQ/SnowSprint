using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Infrastructure.SceneManagement;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State.Payload;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Features.AppStates
{
    public class LoadSceneState : BasePayloadedState<string>
    {
        private readonly ISceneLoader _sceneLoader;
        
        public LoadSceneState(ISceneLoader sceneLoader, IStateMachine stateMachine) : base(stateMachine) 
            => _sceneLoader = sceneLoader;
        
        public override async UniTask OnEnter()
        {
            var nm = NetworkManager.Singleton;
            bool isNetworkActive = nm != null && (nm.IsServer || nm.IsClient);

            if (isNetworkActive && Payload != SceneNames.MainMenu && nm.IsServer) 
                nm.SceneManager.LoadScene(Payload, LoadSceneMode.Single);
            else
                await _sceneLoader.Load(Payload);
        }
    }
}