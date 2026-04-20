using _Project.Scripts.Features.Services;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Features.UI.HUD
{
    public class LevelProgressView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Transform finishTrigger; 
        [SerializeField] private Transform playerTransform;

        private LevelProgressCalculator _calculator;
        private int _lastPercent = -1;

        private void Start()
        {
            _calculator = new LevelProgressCalculator(playerTransform.position.x, finishTrigger.position.x);
        }

        private void Update()
        {
            int percent = _calculator.CalculateProgress(playerTransform.position.x);
        
            if (percent != _lastPercent)
            {
                progressText.text = $"{percent}%";
                _lastPercent = percent;
            }
        }
    }
}