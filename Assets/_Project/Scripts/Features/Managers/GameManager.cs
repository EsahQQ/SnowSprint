using System;
using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Gameplay.Player;
using _Project.Scripts.Features.Services;
using _Project.Scripts.Features.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.Scripts.Features.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int levelReward = 100;
        
        private IPlayerDataService _playerData;
        private FinishTrigger _finishTrigger;
        private PlayerController _player; 
        
        public GameState CurrentState { get; private set; }
        public event Action OnLevelStarted;
        public event Action OnLevelFinished;
        
        [Inject]
        public void Construct(IPlayerDataService playerData, FinishTrigger finishTrigger, PlayerController player)
        {
            _playerData = playerData;
            _finishTrigger = finishTrigger;
            _player = player;
            
            _finishTrigger.OnPlayerFinished += FinishLevel;
        }

        private void Start()
        {
            StartLevel();
        }

        private void StartLevel()
        {
            CurrentState = GameState.Gameplay;
            _player.SetActive(true);
            OnLevelStarted?.Invoke();
        }

        private void FinishLevel()
        {
            if (CurrentState != GameState.Gameplay) return;

            CurrentState = GameState.Shop;
            _player.SetActive(false); 

            _playerData.AddCoins(levelReward);
            OnLevelFinished?.Invoke();
        }

        public void RestartLevel()
        {
            DOTween.KillAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}