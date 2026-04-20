using _Project.Scripts.Features.Gameplay.Player.PlayerInput;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        private IPlayerInput _input; 
        private PlayerPhysics _physics;
        private bool _isActive = true; 

        [Inject]
        public void Construct(IPlayerInput input)
        {
            _input = input;
        }

        private void Awake()
        {
            _physics = GetComponent<PlayerPhysics>();
        }

        private void Update()
        {
            if (!_isActive) return;

            if (_input.GetJumpInput()) 
                _physics.Jump();
            
            if (_input.GetBoostInput())
                _physics.Boost();
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            _physics.SetActive(isActive);
        }
    }
}