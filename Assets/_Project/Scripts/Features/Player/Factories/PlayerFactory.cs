using _Project.Scripts.Features.Player.Provider;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Player.Factories
{
    public class PlayerFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly IPlayerProvider _playerProvider;
        private readonly PlayerController _playerPrefab;

        public PlayerFactory(IInstantiator instantiator, IPlayerProvider playerProvider, PlayerController playerPrefab)
        {
            _instantiator = instantiator;
            _playerProvider = playerProvider;
            _playerPrefab = playerPrefab;
        }

        public PlayerController Create(Transform spawnPoint)
        {
            var player = _instantiator.InstantiatePrefabForComponent<PlayerController>(_playerPrefab, spawnPoint.position, Quaternion.identity, null);
            _playerProvider.RegisterPlayer(player);
            
            return player;
        }
    }
}