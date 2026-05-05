using System;
using _Project.Scripts.Features.Network.Gameplay;
using _Project.Scripts.Features.Network.Lobby;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.UI;
using _Project.Scripts.Features.UI.HUD;
using _Project.Scripts.Features.UI.Shop;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Gameplay
{
    public class GameController : IInitializable, IDisposable, ITickable
    {
        private readonly IPlayerProvider _playerProvider;
        private readonly IHudView _hudView;
        private readonly IShopView _shopView;
        private readonly LevelProgressView _levelProgressView;
        private readonly IPlayerDataService _playerDataService;
        private readonly LobbyController _lobbyController;

        public GameController(
            IPlayerProvider playerProvider,
            IHudView hudView,
            IShopView shopView,
            LevelProgressView levelProgressView,
            IPlayerDataService playerDataService,
            LobbyController lobbyController)
        {
            _playerProvider = playerProvider;
            _hudView = hudView;
            _shopView = shopView;
            _levelProgressView = levelProgressView;
            _playerDataService = playerDataService;
            _lobbyController = lobbyController;
        }

        public void Initialize()
        {
            _playerProvider.OnLocalPlayerRegistered += OnLocalPlayerRegistered;
            NetworkClient.RegisterHandler<RaceFinishMessage>(OnRaceFinishReceived);
        }

        public void Dispose()
        {
            _playerProvider.OnLocalPlayerRegistered -= OnLocalPlayerRegistered;
            
            if (_playerProvider.LocalPlayer != null)
                _playerProvider.LocalPlayer.OnRaceStateChangedEvent -= HandleRaceStateChanged;

            NetworkClient.UnregisterHandler<RaceFinishMessage>();
        }
        
        public void Tick() => _levelProgressView.Update();

        private void OnLocalPlayerRegistered(PlayerController player)
        {
            // Подписываемся на изменение состояния (Гонка началась/закончилась)
            player.OnRaceStateChangedEvent += HandleRaceStateChanged;
            
            // Если игрок подключился, а гонка уже идет
            if (player.IsRaceActive) HandleRaceStateChanged(true);
        }

        private void HandleRaceStateChanged(bool isActive)
        {
            if (isActive)
            {
                // Гонка началась!
                _lobbyController.HideLobby();
                _hudView.Show();
                _levelProgressView.Init();
            }
        }

        private void OnRaceFinishReceived(RaceFinishMessage msg)
        {
            HandleFinishAsync(msg.CoinsEarned).Forget();
        }

        private async UniTaskVoid HandleFinishAsync(int coinsEarned)
        {
            _playerDataService.AddCoins(coinsEarned); 
            _hudView.Hide(); 
            
            await _shopView.ProcessShopAsync(); 
            
            if (NetworkClient.isConnected)
            {
                // 1. Говорим серверу, что мы закончили (чтобы он нас телепортировал/очистил)
                NetworkClient.Send(new ReturnToLobbyMessage());
                
                // 2. ЗАГРУЖАЕМ ГЛАВНОЕ МЕНЮ
                // Это уничтожит текущий LobbyController и при следующем входе всё будет 0/0
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
    }
}