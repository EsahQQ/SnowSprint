using System;
using System.Collections.Generic;
using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Network;
using _Project.Scripts.Features.Network.Gameplay;
using _Project.Scripts.Features.Network.Server.Auth;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.Player.Services;
using _Project.Scripts.Features.SceneConstants;
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
        private readonly FinishTrigger _finishTrigger;
        private readonly LevelProgressView _levelProgressView;
        private readonly IPlayerDataService _playerDataService;
        private readonly Transform _spawnPoint;
        private readonly IRaceRewardService _raceRewardService;
        private readonly List<PlayerController> _serverSpawnedPlayers = new();

        private const int CoinsPerRaceWin = 50;
        private bool _raceFinished;

        public GameController(
            IPlayerProvider playerProvider,
            IHudView hudView,
            IShopView shopView,
            FinishTrigger finishTrigger,
            LevelProgressView levelProgressView,
            IPlayerDataService playerDataService,
            [Inject(Id = "SpawnPoint")] Transform spawnPoint,
            IRaceRewardService raceRewardService)
        {
            _playerProvider = playerProvider;
            _hudView = hudView;
            _shopView = shopView;
            _finishTrigger = finishTrigger;
            _levelProgressView = levelProgressView;
            _playerDataService = playerDataService;
            _spawnPoint = spawnPoint;
        }

        public void Initialize()
        {
            _playerProvider.OnLocalPlayerRegistered += OnLocalPlayerRegistered;
            _playerProvider.OnAnyPlayerRegistered += OnAnyPlayerRegistered; 
    
            _finishTrigger.OnPlayerFinished += OnPlayerFinished;
            NetworkClient.RegisterHandler<RaceFinishMessage>(OnRaceFinishReceived);
        }

        public void Dispose()
        {
            _playerProvider.OnLocalPlayerRegistered -= OnLocalPlayerRegistered;
            _playerProvider.OnAnyPlayerRegistered -= OnAnyPlayerRegistered;
            _finishTrigger.OnPlayerFinished -= OnPlayerFinished;
            NetworkClient.UnregisterHandler<RaceFinishMessage>();
        }
        
        public void Tick() => _levelProgressView.Update();
        
        private void OnLocalPlayerRegistered(PlayerController player)
        {
            _hudView.Show();
            _levelProgressView.Init();
        }

        private void OnAnyPlayerRegistered(PlayerController player)
        {
            Debug.Log($"[GameController] Зарегистрирован игрок. isServer: {NetworkServer.active}, isClient: {NetworkClient.active}");
            if (NetworkServer.active)
            {
                player.Initialize();
                player.transform.position = _spawnPoint != null ? _spawnPoint.position : Vector3.zero;
                player.SetActive(true);
                Debug.Log("[GameController] Сервер активировал гонку для игрока (IsRaceActive = true)");
            }
        }

        private void OnPlayerFinished()
        {
            if (!NetworkServer.active || _raceFinished) return;
            _raceFinished = true;

            foreach (var player in _playerProvider.AllPlayers)
                player.SetActive(false);
            
            _raceRewardService.GrantCoinsToAllPlayers(CoinsPerRaceWin);

            NetworkServer.SendToAll(new RaceFinishMessage { CoinsEarned = CoinsPerRaceWin });
        }

        private void OnRaceFinishReceived(RaceFinishMessage msg)
        {
            HandleFinishAsync(msg.CoinsEarned).Forget();
        }

        private async UniTaskVoid HandleFinishAsync(int coinsEarned)
        {
            Debug.Log($"[GameController] Получено монет за финиш: {coinsEarned}");
            _playerDataService.AddCoins(coinsEarned);
            await _shopView.ProcessShopAsync();
            
            _hudView.Hide();

            if (NetworkServer.active && NetworkClient.isConnected)
                NetworkManager.singleton.ServerChangeScene(SceneNames.LobbyMenu);
            else if (NetworkClient.isConnected)
                NetworkManager.singleton.StopClient();
        }
    }
}
