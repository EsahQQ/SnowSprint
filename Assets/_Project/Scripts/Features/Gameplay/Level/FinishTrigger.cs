using System;
using _Project.Scripts.Features.Player;
using Mirror;
using UnityEngine;

namespace _Project.Scripts.Features.Gameplay.Level
{
    public class FinishTrigger : MonoBehaviour
    {
        public event Action<PlayerController> OnPlayerFinished;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!NetworkServer.active) return;

            // 2. Ищем компонент игрока
            if (other.TryGetComponent(out PlayerController player))
            {
                Debug.Log($"[FinishTrigger] СЕРВЕР ЗАРЕГИСТРИРОВАЛ ФИНИШ! Игрок: {player.gameObject.name}");
                OnPlayerFinished?.Invoke(player);
            }
        }
    }
}