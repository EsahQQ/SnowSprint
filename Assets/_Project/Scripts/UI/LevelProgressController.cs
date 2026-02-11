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

        private void Start()
        {
            if (finishTrigger == null || playerTransform == null) return;
            
            _finishX = finishTrigger.position.x;
            _totalDistance = _finishX - _startX;
        }

        private void Update()
        {
            if (playerTransform == null || finishTrigger == null) return;

            var currentDistance = playerTransform.position.x - _startX;
            var progress = Mathf.Clamp01(currentDistance / _totalDistance);

            progressText.text = $"{Mathf.FloorToInt(progress * 100)}%";
        }
    }
}