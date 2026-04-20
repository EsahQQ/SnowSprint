using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Gameplay.Player;
using _Project.Scripts.Features.Gameplay.Player.PlayerInput;
using _Project.Scripts.Features.Managers;
using _Project.Scripts.Features.Services;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameManager gameManagerPrefab;
        [SerializeField] private FinishTrigger finishTrigger;
        [SerializeField] private ShopDatabase shopDatabase;
        
        [Header("Player Refs (Для синглплеера/прототипа)")]
        [SerializeField] private PlayerController localPlayer;
        [SerializeField] private LocalPlayerInput localInput;

        public override void InstallBindings()
        {
            Container.Bind<IPlayerDataService>().To<PlayerDataService>().AsSingle();
            Container.Bind<ShopDatabase>().FromInstance(shopDatabase).AsSingle();
            Container.Bind<FinishTrigger>().FromComponentInHierarchy(finishTrigger).AsSingle();
            Container.Bind<IPlayerInput>().FromInstance(localInput).AsSingle();
            Container.Bind<PlayerController>().FromInstance(localPlayer).AsSingle();
            Container.Bind<GameManager>().FromComponentInNewPrefab(gameManagerPrefab).AsSingle().NonLazy();
        }
    }
}