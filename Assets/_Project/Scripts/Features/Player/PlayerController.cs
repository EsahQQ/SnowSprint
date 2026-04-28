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
        [SerializeField] private Rigidbody2D _rb;

        private IPlayerProvider _playerProvider;
        
        public NetworkVariable<bool> IsRaceActive = new NetworkVariable<bool>();
        
        private float _serverMaxSpeed;
        private float _serverAcceleration;
        private float _serverBoostForce;
        private float _serverJumpForce;

        [Inject]
        public void Construct(IPlayerProvider playerProvider) => _playerProvider = playerProvider;

        public override void OnNetworkSpawn()
        {
            _playerProvider.RegisterPlayer(this);
            
            _rb.bodyType = IsServer ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
            _rb.simulated = true;

            if (IsOwner)
            {
                _stats.Initialize();
                SubmitStatsServerRpc(
                    _stats.CurrentMaxSpeed, 
                    _stats.CurrentAcceleration, 
                    _stats.CurrentBoostForce, 
                    _stats.CurrentJumpForce);
            }
        }

        public override void OnNetworkDespawn() => _playerProvider?.UnregisterPlayer(this);

        public void Initialize()
        {
            _physics.Initialize();
            _visuals.Initialize();
        }
        
        public void Tick()
        {
            if (!IsRaceActive.Value) return;
            
            if (IsOwner)
            {
                _input.Tick();
            
                if (_input.GetJumpInput()) 
                    RequestJumpServerRpc();
            
                if (_input.GetBoostInput())
                    RequestBoostServerRpc();
            }
        }
        
        public void FixedTick(float fixedDt)
        {
            if (!IsRaceActive.Value) return;
            
            if (IsServer)
                _physics.FixedTick(_serverMaxSpeed, _serverAcceleration);
            
            _physics.CheckGround(); 
            _visuals.FixedTick(fixedDt, _physics.GroundNormal);
        }

        public void SetActive(bool isActive)
        {
            if (IsServer)
            {
                IsRaceActive.Value = isActive;
                _physics.ResetVelocity();
            }
        }

        #region Server RPCs

        [ServerRpc(RequireOwnership = true)]
        private void SubmitStatsServerRpc(float maxSpeed, float acceleration, float boost, float jump)
        {
            _serverMaxSpeed = maxSpeed;
            _serverAcceleration = acceleration;
            _serverBoostForce = boost;
            _serverJumpForce = jump;
        }

        [ServerRpc(RequireOwnership = true)]
        private void RequestJumpServerRpc()
        {
            if (!IsRaceActive.Value) return;
            _physics.Jump(_serverJumpForce);
        }

        [ServerRpc(RequireOwnership = true)]
        private void RequestBoostServerRpc()
        {
            if (!IsRaceActive.Value) return;
            _physics.Boost(_serverBoostForce);
        }

        #endregion
    }
}