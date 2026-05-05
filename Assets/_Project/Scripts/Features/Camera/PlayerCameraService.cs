using System;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Features.Player.Provider;
using Unity.Cinemachine;
using Zenject;

namespace _Project.Scripts.Features.Camera
{
    public class PlayerCameraService : IInitializable, IDisposable
    {
        private readonly IPlayerProvider _playerProvider;
        private readonly CinemachineCamera _virtualCamera;

        public PlayerCameraService(IPlayerProvider playerProvider, CinemachineCamera virtualCamera)
        {
            _playerProvider = playerProvider;
            _virtualCamera = virtualCamera;
        }

        public void Initialize()
        {
            if (_playerProvider.LocalPlayer != null)
                SetupCamera(_playerProvider.LocalPlayer);
            else
                _playerProvider.OnLocalPlayerRegistered += SetupCamera;
        }

        public void Dispose()
        {
            _playerProvider.OnLocalPlayerRegistered -= SetupCamera;
        }

        private void SetupCamera(PlayerController player)
        {
            _playerProvider.OnLocalPlayerRegistered -= SetupCamera;
            _virtualCamera.Follow = player.transform;
            _virtualCamera.LookAt = player.transform;
        }
    }
}