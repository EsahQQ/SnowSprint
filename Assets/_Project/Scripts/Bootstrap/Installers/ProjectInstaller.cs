using _Project.Scripts.Features.Network.Lobby;
using _Project.Scripts.Features.UI.Shop.Settings;
using _Project.Scripts.Infrastructure.SceneManagement;
using _Project.Scripts.Features.Network.Server.Email;
using _Project.Scripts.Features.Network.Server.ServerDatabase;
using Mirror;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private NetworkManager _networkManagerPrefab; 
        [SerializeField] private LobbyNetworkManager _lobbyNetworkManagerPrefab;
        [SerializeField] private ShopDatabase _shopDatabase;
        
        public override void InstallBindings()
        {
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            
            Container.Bind<IServerDatabase>().To<LocalJsonDatabase>().AsSingle();
            Container.Bind<IEmailService>().To<SmtpEmailService>().AsSingle();
            
            Container.Bind<NetworkManager>()
                .FromComponentInNewPrefab(_networkManagerPrefab)
                .AsSingle()
                .NonLazy(); 
            
            Container.Bind<LobbyNetworkManager>()
                .FromComponentInNewPrefab(_lobbyNetworkManagerPrefab)
                .AsSingle()
                .NonLazy(); 
            
            Container.Bind<ShopDatabase>().FromInstance(_shopDatabase).AsSingle();
        }
    }
}