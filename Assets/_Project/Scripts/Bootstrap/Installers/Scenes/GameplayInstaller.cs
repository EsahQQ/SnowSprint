using _Project.Scripts.Bootstrap.EntryPoint;
using _Project.Scripts.Bootstrap.InitPipeline.Initializers.Scene;
using _Project.Scripts.Features.AppStates.SetupStates;
using _Project.Scripts.Features.UI;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers.Scenes
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("UI Views")]
        [SerializeField] private HudView hudView;
        [SerializeField] private ShopView shopView;
        
        public override void InstallBindings()
        {
            Container.Bind<ISceneInitializer>().To<NullSceneInitializer>().AsSingle();
            Container.BindInterfacesAndSelfTo<SceneEntryPoint<LevelSetupState>>().AsSingle();
            
            Container.Bind<IHudView>().FromInstance(hudView).AsSingle();
            Container.Bind<IShopView>().FromInstance(shopView).AsSingle();
        }
    }
}