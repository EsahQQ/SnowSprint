using System.Collections.Generic;
using _Project.Scripts.Features.Gameplay.Level;
using _Project.Scripts.Features.Network.Server.Auth;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Utils;
using Mirror;
using UnityEngine;

namespace _Project.Scripts.Features.Network.Gameplay
{
    public class LevelInstance : NetworkBehaviour
    {
        [SerializeField] public Transform SpawnPoint;
        [SerializeField] public FinishTrigger FinishTrigger;
    
        [SyncVar] public string MatchId;
        
        private readonly List<NetworkConnectionToClient> _matchConnections = new();
        private bool _isMatchFinished = false;
        private const int CoinsPerRaceWin = 50;

        public void Setup(string matchId, List<NetworkConnectionToClient> players)
        {
            MatchId = matchId;
            var matchGuid = matchId.ToGuid();

            GetComponent<NetworkMatch>().matchId = matchGuid;
            FinishTrigger.GetComponent<NetworkMatch>().matchId = matchGuid;
            
            _matchConnections.AddRange(players);

            // Убрали проверку isServer! Метод Setup и так вызывается только на сервере.
            FinishTrigger.OnPlayerFinished += HandlePlayerFinished;
            Debug.Log($"[LevelInstance] Уровень настроен. Подписка на финиш оформлена.");
        }

        [ServerCallback]
        private void HandlePlayerFinished(PlayerController winner)
        {
            if (_isMatchFinished) return;
            _isMatchFinished = true;

            Debug.Log($"[LevelInstance] ОБРАБОТКА ФИНИША! Выдаем награды...");

            var rewardService = NetworkManager.singleton.authenticator as IRaceRewardService;

            foreach (var conn in _matchConnections)
            {
                if (conn.identity != null && conn.identity.TryGetComponent(out PlayerController player))
                    player.SetActive(false); // Останавливаем физику
                
                if (rewardService != null)
                {
                    rewardService.GrantCoinsToPlayer(conn, CoinsPerRaceWin);
                    Debug.Log($"[LevelInstance] Выдано {CoinsPerRaceWin} монет соединению {conn.connectionId}");
                }
                
                conn.Send(new RaceFinishMessage { CoinsEarned = CoinsPerRaceWin });
            }

            Invoke(nameof(DestroyLevel), 10f);
        }

        [ServerCallback]
        private void DestroyLevel() => NetworkServer.Destroy(gameObject);

        private void OnDestroy()
        {
            if (FinishTrigger != null)
                FinishTrigger.OnPlayerFinished -= HandlePlayerFinished;
        }
    }
}