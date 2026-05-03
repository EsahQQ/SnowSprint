using _Project.Scripts.Features.Player.PlayerInput;
using _Project.Scripts.Features.Player.Provider;
using Mirror;
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
        
        [SyncVar] public bool IsRaceActive;
        
        private float _serverMaxSpeed;
        private float _serverAcceleration;
        private float _serverBoostForce;
        private float _serverJumpForce;

        [Inject]
        public void Construct(IPlayerProvider playerProvider) => _playerProvider = playerProvider;
        
        public override void OnStartClient()
        {
            _playerProvider.RegisterPlayer(this);
            _rb.simulated = true;
        }
        
        public override void OnStartServer()
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }

        public override void OnStartLocalPlayer()
        {
            _stats.Initialize();
            CmdSubmitStats(
                _stats.CurrentMaxSpeed, 
                _stats.CurrentAcceleration, 
                _stats.CurrentBoostForce, 
                _stats.CurrentJumpForce);
        }

        public override void OnStopClient() => _playerProvider?.UnregisterPlayer(this);

        public void Initialize()
        {
            _physics.Initialize();
            _visuals.Initialize();
        }
        
        public void Tick()
        {
            if (!IsRaceActive) return;
            
            if (isLocalPlayer)
            {
                _input.Tick();
            
                if (_input.GetJumpInput()) 
                    CmdRequestJump();
            
                if (_input.GetBoostInput())
                    CmdRequestBoost();
            }
        }
        
        public void FixedTick(float fixedDt)
        {
            if (!IsRaceActive) return;
            
            if (isServer)
                _physics.FixedTick(_serverMaxSpeed, _serverAcceleration);
            
            _physics.CheckGround(); 
            _visuals.FixedTick(fixedDt, _physics.GroundNormal);
        }

        public void SetActive(bool isActive)
        {
            if (isServer)
            {
                IsRaceActive = isActive;
                _physics.ResetVelocity();
            }
        }

        #region Commands (Вызовы от Клиента к Серверу)

        [Command]
        private void CmdSubmitStats(float maxSpeed, float acceleration, float boost, float jump)
        {
            _serverMaxSpeed = maxSpeed;
            _serverAcceleration = acceleration;
            _serverBoostForce = boost;
            _serverJumpForce = jump;
        }

        [Command]
        private void CmdRequestJump()
        {
            if (!IsRaceActive) return;
            _physics.Jump(_serverJumpForce);
        }

        [Command]
        private void CmdRequestBoost()
        {
            if (!IsRaceActive) return;
            _physics.Boost(_serverBoostForce);
        }

        #endregion
    }
}