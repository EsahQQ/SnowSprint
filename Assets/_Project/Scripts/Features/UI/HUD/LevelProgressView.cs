using _Project.Scripts.Features.Player.Provider;
using _Project.Scripts.Features.Services;
using TMPro;

namespace _Project.Scripts.Features.UI.HUD
{
    public class LevelProgressView
    {
        private readonly TextMeshProUGUI _progressText;
        private readonly IPlayerProvider _playerProvider;
        private LevelProgressCalculator _calculator;
        private int _lastPercent = -1;
        
        // Убрали FinishTrigger из конструктора!
        public LevelProgressView(IPlayerProvider playerProvider, TextMeshProUGUI progressText)
        {
            _playerProvider = playerProvider;
            _progressText = progressText;
        }

        public void Init()
        {
            if (_playerProvider.LocalPlayer == null) return;
            float startX = _playerProvider.LocalPlayer.transform.position.x;
            // Укажи здесь примерную длину твоего уровня по оси X (допустим 100)
            float fakeFinishX = startX + 750f; 
            _calculator = new LevelProgressCalculator(startX, fakeFinishX);
        }
        
        public void Update()
        {
            if (_calculator == null || _playerProvider.LocalPlayer == null) return;

            var percent = _calculator.CalculateProgress(_playerProvider.LocalPlayer.transform.position.x);
        
            if (percent != _lastPercent)
            {
                _progressText.text = $"{percent}%";
                _lastPercent = percent;
            }
        }
    }
}