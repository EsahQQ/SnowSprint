using _Project.Scripts.Features.Camera;
using _Project.Scripts.Features.Gameplay;
using _Project.Scripts.Features.Network.Lobby;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.Player.Settings;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Features.UI.HUD;
using _Project.Scripts.Features.UI.Lobby;
using _Project.Scripts.Features.UI.Shop;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers.Scenes
{
    public class OnlineSceneInstaller : MonoInstaller
    {
        [Header("UI Views (Canvas)")]
        [SerializeField] private LobbyView _lobbyView;
        [SerializeField] private HudView _hudView;
        [SerializeField] private ShopView _shopView;
        
        [Tooltip("Текст процентов на HUD'е")]
        [SerializeField] private TextMeshProUGUI _progressText; 

        [Header("Camera")]
        [SerializeField] private CinemachineCamera _virtualCamera;

        public override void InstallBindings()
        {
            
            
            // 3. UI интерфейсы
            Container.Bind<ILobbyView>().FromInstance(_lobbyView).AsSingle();
            Container.Bind<IHudView>().FromInstance(_hudView).AsSingle();
            Container.Bind<IShopView>().FromInstance(_shopView).AsSingle();
        
            // Передаем ссылку на текст прямо в конструктор LevelProgressView
            Container.Bind<LevelProgressView>().AsSingle().WithArguments(_progressText);

            // 4. Камера
            Container.Bind<CinemachineCamera>().FromInstance(_virtualCamera).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerCameraService>().AsSingle();

            // 5. Контроллеры логики
            Container.BindInterfacesAndSelfTo<LobbyController>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
        }
    }
}