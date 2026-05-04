using Mirror;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Player.Factories
{
    public class NetworkPlayerFactory : Libs.Factories.IFactory<PlayerController>
    {
        private readonly DiContainer _container;
        private readonly PlayerController _playerPrefab;

        public NetworkPlayerFactory(DiContainer container, PlayerController playerPrefab)
        {
            _container = container;
            _playerPrefab = playerPrefab;
            
            RegisterClientSpawnHandler();
        }

        private void RegisterClientSpawnHandler()
        {
            if (!NetworkClient.active)
            {
                Debug.Log("[NetworkPlayerFactory] NetworkClient не активен — пропускаем регистрацию (сервер)");
                return;
            }
    
            var netIdentity = _playerPrefab.GetComponent<NetworkIdentity>();
            if (netIdentity == null)
            {
                Debug.LogError("[NetworkPlayerFactory] На префабе нет NetworkIdentity!");
                return;
            }

            var assetId = netIdentity.assetId;
            Debug.Log($"[NetworkPlayerFactory] Регистрируем SpawnHandler, assetId={assetId}");
    
            NetworkClient.RegisterSpawnHandler(
                assetId,
                spawnMsg =>
                {
                    Debug.Log("[NetworkPlayerFactory] SpawnHandler вызван — инжектируем...");
                    var instance = Object.Instantiate(_playerPrefab);
                    _container.InjectGameObject(instance.gameObject);
                    return instance.gameObject;
                },
                unspawnedObj => Object.Destroy(unspawnedObj)
            );
        }

        public PlayerController Create()
        {
            var instance = Object.Instantiate(_playerPrefab);
            _container.InjectGameObject(instance.gameObject);
            return instance;
        }
    }
}