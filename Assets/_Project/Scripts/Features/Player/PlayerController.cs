using _Project.Scripts.Features.Player.PlayerInput;
using UnityEngine;

namespace _Project.Scripts.Features.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private AbstractPlayerInput _input; 
        [SerializeField] private PlayerPhysics _physics;
        [SerializeField] private PlayerStatsHandler _stats;
        [SerializeField] private PlayerVisuals _visuals;

        private bool _isActive;
        
        public void Initialize()
        {
            _stats.Initialize();
            _physics.Initialize();
            _visuals.Initialize();
        }
        
        public void Tick()
        {
            if (!_isActive) return;
            
            _input.Tick();
            
            if (_input.GetJumpInput()) 
                _physics.Jump(_stats.CurrentJumpForce);
            
            if (_input.GetBoostInput())
                _physics.Boost(_stats.CurrentBoostForce);
        }
        
        public void FixedTick(float fixedDt)
        {
            if (!_isActive) return;
            
            _physics.FixedTick(_stats.CurrentMaxSpeed, _stats.CurrentAcceleration);
            
            _visuals.FixedTick(fixedDt, _physics.GroundNormal);
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            _physics.SetActive(isActive);
        }
    }
}