using _Project.Scripts.Features.Network;
using _Project.Scripts.Features.Network.Lobby;
using _Project.Scripts.Features.Network.Server.Auth;
using _Project.Scripts.Features.Network.Server.Email;
using _Project.Scripts.Features.Network.Server.ServerDatabase;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.Player.Settings;
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
        [SerializeField] private LobbyNetworkManager _lobbyManagerPrefab; 
        [SerializeField] private ShopDatabase _shopDatabase;
        [Header("Settings")]
        [SerializeField] private PlayerSettings _playerSettings;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerSettings>().FromInstance(_playerSettings).AsSingle();
            
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            Container.Bind<IServerDatabase>().To<LocalJsonDatabase>().AsSingle();
            Container.Bind<IEmailService>().To<SmtpEmailService>().AsSingle();
            Container.Bind<ShopDatabase>().FromInstance(_shopDatabase).AsSingle();
            Container.BindInterfacesAndSelfTo<ClientPlayerDataService>().AsSingle();
            
            var networkManager = Container.InstantiatePrefab(_networkManagerPrefab);
            Container.Bind<NetworkManager>()
                .FromInstance(networkManager.GetComponent<GameNetworkManager>())
                .AsSingle();
            
            var lobbyManager = Container.InstantiatePrefab(_lobbyManagerPrefab);
            
            Container.Bind<IPlayerProvider>().To<PlayerProvider>().AsSingle();
            
            Container.Inject(lobbyManager.GetComponent<LobbyNetworkManager>());

            Container.Bind<LobbyNetworkManager>()
                .FromInstance(lobbyManager.GetComponent<LobbyNetworkManager>())
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