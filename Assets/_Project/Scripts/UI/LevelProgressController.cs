using UnityEngine;
using TMPro;
using Zenject;
using _Project.Scripts.Core;

namespace _Project.Scripts.UI
{
    public class LevelProgressController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Transform finishTrigger; 
        [SerializeField] private Transform playerTransform;
        
        private float _startX;
        private float _finishX;
        private float _totalDistance;
        private int _lastPercent = 0;

        private void Start()
        {
            if (finishTrigger == null || playerTransform == null) return;
            
            _startX = playerTransform.position.x;
            _finishX = finishTrigger.position.x;
            _totalDistance = _finishX - _startX;
        }

        private void Update()
        {
            if (playerTransform == null || finishTrigger == null) return;

            var currentDistance = playerTransform.position.x - _startX;
            var progress = Mathf.Clamp01(currentDistance / _totalDistance);
            
            var percent = Mathf.FloorToInt(progress * 100);
            if (percent == _lastPercent) return;

            progressText.text = $"{percent}%";
            _lastPercent = percent;
        }
    }
}