using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.Network
{
    public class MockNetworkSessionService : INetworkSessionService
    {
        public bool IsHost { get; private set; }
        public bool IsConnected { get; private set; }
        public event Action OnAllPlayersReady;

        public void StartHost()
        {
            IsHost = true;
            IsConnected = true;
            Debug.Log("[Network] Хост запущен. Ожидание игроков...");
        }

        public void StartClient()
        {
            IsHost = false;
            IsConnected = true;
            Debug.Log("[Network] Клиент подключился к лобби.");
        }

        public async void SetLocalPlayerReady(bool isReady)
        {
            Debug.Log($"[Network] Игрок готов: {isReady}");
            
            if (isReady)
            {
                await UniTask.Delay(2000);
                Debug.Log("[Network] Все игроки готовы! Запускаем игру...");
                OnAllPlayersReady?.Invoke();
            }
        }
        
        public async UniTask QuickJoinOrCreateAsync()
        {
            Debug.Log("[Network] Поиск доступных серверов...");
            
            await UniTask.Delay(1500); 
            
            bool foundExistingLobby = Random.value > 0.5f;

            if (foundExistingLobby)
            {
                Debug.Log("[Network] Сервер найден! Подключаемся...");
                StartClient();
            }
            else
            {
                Debug.Log("[Network] Нет доступных серверов. Создаем свой...");
                StartHost();
            }
        }
    }
}