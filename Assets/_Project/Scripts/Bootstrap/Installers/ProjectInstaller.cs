using _Project.Scripts.Features.Network;
using _Project.Scripts.Features.Network.Server.Auth;
using _Project.Scripts.Features.Network.Server.Email;
using _Project.Scripts.Features.Network.Server.ServerDatabase;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.UI.Shop.Settings;
using _Project.Scripts.Infrastructure.SceneManagement;
using Mirror;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameNetworkManager _networkManagerPrefab;
        [SerializeField] private ShopDatabase _shopDatabase;
        
        public override void InstallBindings()
        {
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            Container.Bind<IServerDatabase>().To<LocalJsonDatabase>().AsSingle();
            Container.Bind<IEmailService>().To<SmtpEmailService>().AsSingle();
            Container.Bind<ShopDatabase>().FromInstance(_shopDatabase).AsSingle();
            Container.BindInterfacesAndSelfTo<ClientPlayerDataService>().AsSingle();
            
            var networkManager = Container.InstantiatePrefab(_networkManagerPrefab);
            Container.Bind<NetworkManager>()
                .FromInstance(networkManager.GetComponent<GameNetworkManager>())
                .AsSingle();

            var authenticator = networkManager.GetComponentInChildren<GameAuthenticator>();
            if (authenticator != null)
            {
                Container.Inject(authenticator);
                Container.Bind<IRaceRewardService>()
                    .FromInstance(authenticator)
                    .AsSingle();
            }
            else
                Debug.LogError("[Installer] GameAuthenticator не найден!");
        }
    }
}