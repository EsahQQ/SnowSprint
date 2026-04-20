using _Project.Scripts.Bootstrap.EntryPoint;
using _Project.Scripts.Bootstrap.InitPipeline.Initializers.Scene;
using _Project.Scripts.Features.AppStates;
using _Project.Scripts.Features.UI;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers.Scenes
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private MainMenuView mainMenuView;
        public override void InstallBindings()
        {
            BindScenePipeline();
            Container.BindInterfacesAndSelfTo<SceneEntryPoint<MainMenuState>>().AsSingle();
            
            Container.Bind<IMainMenuView>().FromInstance(mainMenuView).AsSingle();
        }

        private void BindScenePipeline()
        {
            Container.Bind<ISceneInitializer>().To<NullSceneInitializer>().AsSingle();
        }
    }
}