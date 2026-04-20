using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.UI.HUD
{
    public class LevelProgressView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI progressText;

        private IPlayerProvider _playerProvider;
        private FinishTrigger _finishTrigger;
        private LevelProgressCalculator _calculator;
        private int _lastPercent = -1;

        [Inject]
        public void Construct(IPlayerProvider playerProvider, FinishTrigger finishTrigger)
        {
            _playerProvider = playerProvider;
            _finishTrigger = finishTrigger;
            _playerProvider.OnPlayerRegistered += SetupCalculator;
        }
        
        private void OnDestroy() => _playerProvider.OnPlayerRegistered -= SetupCalculator;
        private void SetupCalculator(PlayerController player) => _calculator = new LevelProgressCalculator(player.transform.position.x, _finishTrigger.transform.position.x);
        
        private void Update()
        {
            if (_calculator == null || _playerProvider.Player == null) return;

            int percent = _calculator.CalculateProgress(_playerProvider.Player.transform.position.x);
        
            if (percent != _lastPercent)
            {
                progressText.text = $"{percent}%";
                _lastPercent = percent;
            }
        }
    }
}