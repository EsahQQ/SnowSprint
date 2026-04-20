using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Player.Services;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private FinishTrigger finishTrigger;
        [SerializeField] private ShopDatabase shopDatabase;

        public override void InstallBindings()
        {
            Container.Bind<IPlayerDataService>().To<PlayerDataService>().AsSingle();
            Container.Bind<ShopDatabase>().FromInstance(shopDatabase).AsSingle();
            Container.Bind<FinishTrigger>().FromComponentInHierarchy(finishTrigger).AsSingle();
        }
    }
}