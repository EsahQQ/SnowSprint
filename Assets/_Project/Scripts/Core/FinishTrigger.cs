using System;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Core
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