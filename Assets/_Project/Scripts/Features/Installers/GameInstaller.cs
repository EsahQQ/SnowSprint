using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.Player.Settings;
using _Project.Scripts.Features.UI.HUD;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private FinishTrigger finishTrigger;
        [SerializeField] private ShopDatabase shopDatabase;
        [SerializeField] private PlayerSettings playerSettings;
        [SerializeField] private TextMeshProUGUI progressText;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerSettings>().FromInstance(playerSettings).AsSingle();
            Container.Bind<IPlayerDataService>().To<PlayerDataService>().AsSingle();
            Container.Bind<ShopDatabase>().FromInstance(shopDatabase).AsSingle();
            Container.Bind<FinishTrigger>().FromComponentInHierarchy(finishTrigger).AsSingle();
            
            Container.Bind<TextMeshProUGUI>().FromComponentInHierarchy(progressText).AsSingle().WhenInjectedInto<LevelProgressView>();
            Container.Bind<LevelProgressView>().AsSingle();
        }
    }
}