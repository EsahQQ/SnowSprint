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
        }

        public PlayerController Create()
        {
            var instance = Object.Instantiate(_playerPrefab);
            _container.InjectGameObject(instance.gameObject);

            return instance;
        }
    }
}