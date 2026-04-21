using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.Player.Settings;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private FinishTrigger finishTrigger;
        [SerializeField] private ShopDatabase shopDatabase;
        [SerializeField] private PlayerSettings playerSettings;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerSettings>().FromInstance(playerSettings).AsSingle();
            Container.Bind<IPlayerDataService>().To<PlayerDataService>().AsSingle();
            Container.Bind<ShopDatabase>().FromInstance(shopDatabase).AsSingle();
            Container.Bind<FinishTrigger>().FromComponentInHierarchy(finishTrigger).AsSingle();
        }
    }
}