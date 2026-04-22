using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.Player.Settings;
using _Project.Scripts.Features.UI.HUD;
using _Project.Scripts.Features.UI.Shop.Settings;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private FinishTrigger _finishTrigger;
        [SerializeField] private ShopDatabase _shopDatabase;
        [SerializeField] private PlayerSettings _playerSettings;
        [SerializeField] private TextMeshProUGUI _progressText;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerSettings>().FromInstance(_playerSettings).AsSingle();
            Container.Bind<IPlayerDataService>().To<PlayerDataService>().AsSingle();
            Container.Bind<ShopDatabase>().FromInstance(_shopDatabase).AsSingle();
            Container.Bind<FinishTrigger>().FromComponentInHierarchy(_finishTrigger).AsSingle();
            
            Container.Bind<TextMeshProUGUI>().FromComponentInHierarchy(_progressText).AsSingle().WhenInjectedInto<LevelProgressView>();
            Container.Bind<LevelProgressView>().AsSingle();
        }
    }
}