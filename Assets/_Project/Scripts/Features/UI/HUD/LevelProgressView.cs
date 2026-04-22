using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.UI.HUD
{
    public class LevelProgressView
    {
        private readonly TextMeshProUGUI _progressText;
        private readonly IPlayerProvider _playerProvider;
        private readonly FinishTrigger _finishTrigger;
        private LevelProgressCalculator _calculator;
        private int _lastPercent = -1;
        
        public LevelProgressView(IPlayerProvider playerProvider, FinishTrigger finishTrigger, TextMeshProUGUI progressText)
        {
            _playerProvider = playerProvider;
            _finishTrigger = finishTrigger;
            _progressText = progressText;
        }

        public void Init() => _calculator = new LevelProgressCalculator(_playerProvider.LocalPlayer.transform.position.x, _finishTrigger.transform.position.x);
        
        public void Update()
        {
            if (_calculator == null || _playerProvider.LocalPlayer == null) return;

            int percent = _calculator.CalculateProgress(_playerProvider.LocalPlayer.transform.position.x);
        
            if (percent != _lastPercent)
            {
                _progressText.text = $"{percent}%";
                _lastPercent = percent;
            }
        }
    }
}