using Unity.Netcode;
using _Project.Scripts.Features.Player.PlayerInput;
using _Project.Scripts.Features.Player.Provider;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private AbstractPlayerInput _input; 
        [SerializeField] private PlayerPhysics _physics;
        [SerializeField] private PlayerStatsHandler _stats;
        [SerializeField] private PlayerVisuals _visuals;

        private IPlayerProvider _playerProvider;
        private bool _isActive;

        [Inject]
        public void Construct(IPlayerProvider playerProvider) => _playerProvider = playerProvider;

        public override void OnNetworkSpawn()
        {
            _playerProvider.RegisterPlayer(this);

            if (IsOwner) return;
            
            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }

        public override void OnNetworkDespawn() => _playerProvider?.UnregisterPlayer(this);

        public void Initialize()
        {
            _stats.Initialize();
            _physics.Initialize();
            _visuals.Initialize();
        }
        
        public void Tick()
        {
            if (!_isActive || !IsOwner) return;
            
            _input.Tick();
            
            if (_input.GetJumpInput()) 
                _physics.Jump(_stats.CurrentJumpForce);
            
            if (_input.GetBoostInput())
                _physics.Boost(_stats.CurrentBoostForce);
        }
        
        public void FixedTick(float fixedDt)
        {
            if (!_isActive) return;
            
            _physics.FixedTick(_stats.CurrentMaxSpeed, _stats.CurrentAcceleration, IsOwner);

            _visuals.FixedTick(fixedDt, _physics.GroundNormal);
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            
            if (IsOwner) 
                _physics.SetActive(isActive); 
        }
    }
}