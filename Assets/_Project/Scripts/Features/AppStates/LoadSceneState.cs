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
            if (NetworkManager.Singleton != null 
                && (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient) 
                && NetworkManager.Singleton.IsServer)
                NetworkManager.Singleton.SceneManager.LoadScene(Payload, LoadSceneMode.Single);
            else
                await _sceneLoader.Load(Payload);
        }
    }
}