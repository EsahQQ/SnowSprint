using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using _Project.Scripts.Utils;
using Zenject;

namespace _Project.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int levelReward = 100;
        
        private IPlayerDataService _playerData;
        private FinishTrigger _finishTrigger;
        
        public Enums.GameState CurrentState { get; private set; }
        public event Action OnLevelStarted;
        public event Action OnLevelFinished;

        [Inject]
        public void Construct(IPlayerDataService playerData, FinishTrigger finishTrigger)
        {
            _playerData = playerData;
            _finishTrigger = finishTrigger;
            _finishTrigger.OnPlayerFinished += FinishLevel;
        }

        private void Start()
        {
            StartLevel();
        }

        private void StartLevel()
        {
            CurrentState = Enums.GameState.Gameplay;
            OnLevelStarted?.Invoke();
            Time.timeScale = 1f;
        }

        private void FinishLevel()
        {
            if (CurrentState != Enums.GameState.Gameplay) return;

            CurrentState = Enums.GameState.Shop;
            Debug.Log($"Level Finished! Reward: {levelReward} coins.");

            _playerData.AddCoins(levelReward);

            OnLevelFinished?.Invoke();

            Time.timeScale = 0f; 
        }

        private void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}