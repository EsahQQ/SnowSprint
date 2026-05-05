using System;
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
        
        public event Action<bool> OnRaceStateChangedEvent;
        
        [SyncVar(hook = nameof(OnRaceStateChanged))] 
        public bool IsRaceActive;
        
        private float _serverMaxSpeed = 10f;
        private float _serverAcceleration = 2f;
        private float _serverJumpForce = 5f; 
        private float _serverBoostForce = 5f;

        [Inject]
        public void Construct(IPlayerProvider playerProvider) => _playerProvider = playerProvider;
        
        private void Awake()
        {
            var context = FindFirstObjectByType<SceneContext>();
            if (context != null)
                context.Container.InjectGameObject(gameObject);
        }
        
        public override void OnStartClient()
        {
            if (!isServer) 
            {
                _rb.bodyType = RigidbodyType2D.Kinematic;
                _rb.simulated = true;
            }

            if (_playerProvider != null)
                _playerProvider.RegisterPlayer(this);
        }


        public override void OnStartLocalPlayer()
        {
            if (_playerProvider != null)
                _playerProvider.SetLocalPlayer(this);

            _stats.Initialize();
            CmdSubmitStats(
                _stats.CurrentMaxSpeed, 
                _stats.CurrentAcceleration, 
                _stats.CurrentBoostForce, 
                _stats.CurrentJumpForce);
        }
        
        public override void OnStartServer()
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.simulated = true;
            _rb.linearVelocity = Vector2.zero; 
    
            _serverMaxSpeed = 25f;
            _serverAcceleration = 15f;
            _serverJumpForce = 12f;
            _serverBoostForce = 15f;
    
            if (_playerProvider != null)
                _playerProvider.RegisterPlayer(this);
        }

        public override void OnStopClient() => _playerProvider?.UnregisterPlayer(this);

        private void OnRaceStateChanged(bool oldVal, bool newVal)
        {
            if (isLocalPlayer)
                OnRaceStateChangedEvent?.Invoke(newVal);
        }
        
        public void Initialize()
        {
            _physics.Initialize();
            _visuals.Initialize();
        }
        
        private void Update()
        {
            Tick();
        }

        private void FixedUpdate() => FixedTick(Time.fixedDeltaTime);
        
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
                
                if (isActive)
                {
                    _rb.bodyType = RigidbodyType2D.Dynamic;
                    _physics.ResetVelocity();
                }
                else
                {
                    _rb.bodyType = RigidbodyType2D.Kinematic;
                    _physics.ResetVelocity();
                }
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
            Debug.Log($"[Player-Server] Получены статы от клиента: Speed={maxSpeed}, Accel={acceleration}");
        }

        [Command]
        private void CmdRequestJump()
        {
            if (!IsRaceActive) return;
            Debug.Log($"[Server] Прыжок! Земля: {_physics.IsGrounded}, Сила: {_serverJumpForce}");
            _physics.Jump(_serverJumpForce);
        }

        [Command]
        private void CmdRequestBoost()
        {
            if (!IsRaceActive) return;
            Debug.Log($"[Server] Буст! Земля: {_physics.IsGrounded}, Сила: {_serverBoostForce}");
            _physics.Boost(_serverBoostForce);
        }

        #endregion
    }
}