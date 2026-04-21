using UnityEngine;

namespace _Project.Scripts.Features.Services
{
    public class LevelProgressCalculator
    {
        private readonly float _startX;
        private readonly float _totalDistance;

        public LevelProgressCalculator(float startX, float finishX)
        {
            _startX = startX;
            _totalDistance = finishX - startX;
        }

        public int CalculateProgress(float currentX)
        {
            var currentDistance = currentX - _startX;
            var progress = Mathf.Clamp01(currentDistance / _totalDistance);
            return Mathf.FloorToInt(progress * 100);
        }
    }
}