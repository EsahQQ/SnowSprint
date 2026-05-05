using _Project.Scripts.Features.Camera;
using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.Player.Settings;
using _Project.Scripts.Features.UI.HUD;
using _Project.Scripts.Features.UI.Shop.Settings;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private FinishTrigger _finishTrigger;
        [SerializeField] private PlayerSettings _playerSettings;
        [SerializeField] private TextMeshProUGUI _progressText;
        
        [Header("Camera")]                                       
        [SerializeField] private CinemachineCamera _virtualCamera; 

        public override void InstallBindings()
        {
            Container.Bind<PlayerSettings>().FromInstance(_playerSettings).AsSingle();
            
            Container.Bind<FinishTrigger>().FromInstance(_finishTrigger).AsSingle();

            Container.Bind<TextMeshProUGUI>()
                .FromInstance(_progressText)
                .AsSingle()
                .WhenInjectedInto<LevelProgressView>();

            Container.Bind<LevelProgressView>().AsSingle();
            
            Container.Bind<CinemachineCamera>()
                .FromInstance(_virtualCamera)
                .AsSingle();
        
            Container.BindInterfacesAndSelfTo<PlayerCameraService>().AsSingle();
        }
    }
}