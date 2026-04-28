using _Project.Scripts.Bootstrap.InitPipeline.Initializers.Global;
using _Project.Scripts.Bootstrap.InitPipeline.Queue;
using _Project.Scripts.Bootstrap.InitPipeline.Tasks;
using _Project.Scripts.Bootstrap.InitPipeline.Tasks.GlobalTasks;
using _Project.Scripts.Features.Network;
using _Project.Scripts.Features.Network.Auth;
using _Project.Scripts.Infrastructure.SceneManagement;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private NetworkManager _networkManagerPrefab; 
        
        public override void InstallBindings()
        {
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            BindInitPipeline();
            
            var networkManagerInstance = Instantiate(_networkManagerPrefab, null, true);
            DontDestroyOnLoad(networkManagerInstance.gameObject);
            Container.Bind<NetworkManager>().FromInstance(networkManagerInstance).AsSingle();
            
            Container.Bind<INetworkSessionService>().To<RelayNetworkSessionService>().AsSingle();
            Container.Bind<IAuthService>().To<UnityAuthService>().AsSingle();
        }

        private void BindInitPipeline()
        {
            Container.Bind<IGlobalInitializer>().To<GlobalInitializer>().AsSingle();
            Container.Bind<IInitQueue>().To<InitQueue>().AsSingle();
            
            Container.Bind<IInitTask>().To<GoodFpsTask>().AsSingle();
        }
    }
}