using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Player.Factories;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Features.UI.HUD;
using _Project.Scripts.Features.UI.Shop;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers.Scenes
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("UI Views")]
        [SerializeField] private HudView _hudView;
        [SerializeField] private ShopView _shopView;
        
        [Header("Player Settings")]
        [SerializeField] private PlayerController _playerPrefab;
        [SerializeField] private Transform _spawnPoint;
        
        public override void InstallBindings()
        {
            Container.Bind<IHudView>().FromInstance(_hudView).AsSingle();
            Container.Bind<IShopView>().FromInstance(_shopView).AsSingle();
            
            Container.Bind<IPlayerProvider>().To<PlayerProvider>().AsSingle();
            
            Container.Bind<Libs.Factories.IFactory<PlayerController>>()
                .To<NetworkPlayerFactory>() 
                .AsSingle()
                .WithArguments(_playerPrefab);
        }
    }
}