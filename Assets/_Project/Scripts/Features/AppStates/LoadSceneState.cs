using _Project.Scripts.Features.SceneConstants;
using _Project.Scripts.Infrastructure.SceneManagement;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.State.Payload;
using Cysharp.Threading.Tasks;
using Mirror;

namespace _Project.Scripts.Features.AppStates
{
    public class LoadSceneState : BasePayloadedState<string>
    {
        private readonly ISceneLoader _sceneLoader;
        
        public LoadSceneState(ISceneLoader sceneLoader, IStateMachine stateMachine) : base(stateMachine) 
            => _sceneLoader = sceneLoader;
        
        public override async UniTask OnEnter()
        {
            if (NetworkServer.active && Payload != SceneNames.MainMenu) 
                NetworkManager.singleton.ServerChangeScene(Payload);
            else
                await _sceneLoader.Load(Payload);
        }
    }
}