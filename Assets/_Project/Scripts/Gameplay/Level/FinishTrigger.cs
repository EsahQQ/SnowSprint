using System;
using _Project.Scripts.Gameplay.Player;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Level
{
    public class FinishTrigger : MonoBehaviour
    {
        public event Action OnPlayerFinished;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController player))
                OnPlayerFinished?.Invoke();
        }
    }
}