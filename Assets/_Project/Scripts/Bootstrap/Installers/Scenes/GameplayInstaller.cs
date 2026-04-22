using _Project.Scripts.Bootstrap.EntryPoint;
using _Project.Scripts.Bootstrap.InitPipeline.Initializers.Scene;
using _Project.Scripts.Features.AppStates.SetupStates;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Player.Factories;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Features.UI.Shop;
using _Project.Scripts.Libs.Factories;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers.Scenes
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("UI Views")]
        [SerializeField] private HudView hudView;
        [SerializeField] private ShopView shopView;
        
        [Header("Player Settings")]
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private Transform spawnPoint;
        
        public override void InstallBindings()
        {
            Container.Bind<ISceneInitializer>().To<NullSceneInitializer>().AsSingle();
            Container.BindInterfacesAndSelfTo<SceneEntryPoint<LevelSetupState>>().AsSingle();
            
            Container.Bind<IHudView>().FromInstance(hudView).AsSingle();
            Container.Bind<IShopView>().FromInstance(shopView).AsSingle();
            
            Container.Bind<IPlayerProvider>().To<PlayerProvider>().AsSingle();
            Container.Bind<Transform>().FromInstance(spawnPoint).AsSingle().WhenInjectedInto<LevelSetupState>();
            
            Container.Bind<Libs.Factories.IFactory<PlayerController>>()
                .To<NetworkPlayerFactory>() 
                .AsSingle()
                .WithArguments(playerPrefab);
        }
    }
}