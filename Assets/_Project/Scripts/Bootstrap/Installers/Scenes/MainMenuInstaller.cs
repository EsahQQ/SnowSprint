using _Project.Scripts.Bootstrap.EntryPoint;
using _Project.Scripts.Bootstrap.InitPipeline.Initializers.Scene;
using _Project.Scripts.Features.AppStates;
using _Project.Scripts.Features.Network.Server.Auth;
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
            Container.Bind<ISceneInitializer>().To<NullSceneInitializer>().AsSingle();
            Container.BindInterfacesAndSelfTo<SceneEntryPoint<MainMenuState>>().AsSingle();

            Container.Bind<IMainMenuView>().FromInstance(_mainMenuView).AsSingle();
            
            Container.BindInterfacesAndSelfTo<ClientAuthController>().AsSingle();
        }
    }
}