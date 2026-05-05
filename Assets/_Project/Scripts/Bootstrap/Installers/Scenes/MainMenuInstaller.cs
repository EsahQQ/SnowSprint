using _Project.Scripts.Features.Network.Lobby;
using _Project.Scripts.Features.Network.Server.Auth.Controller;
using _Project.Scripts.Features.Player.Settings;
using _Project.Scripts.Features.UI.Menu;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers.Scenes
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private MainMenuView _mainMenuView;

        public override void InstallBindings()
        {
            Container.Bind<IMainMenuView>().FromInstance(_mainMenuView).AsSingle();
            Container.Bind<MainMenuView>().FromInstance(_mainMenuView).AsSingle(); 
            Container.BindInterfacesAndSelfTo<ClientAuthController>().AsSingle();
            
            // Новый контроллер
            Container.BindInterfacesAndSelfTo<LobbyBrowserController>().AsSingle();
        }
    }
}