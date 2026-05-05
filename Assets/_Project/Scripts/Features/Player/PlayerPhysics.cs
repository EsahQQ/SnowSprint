using _Project.Scripts.Features.Player.Settings;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;
        [Inject] private PlayerSettings _playerSettings;
        
        public bool IsGrounded { get; private set; }
        public Vector2 GroundNormal { get; private set; }

        public void Initialize() { }
        
        public void FixedTick(float currentMaxSpeed, float currentAcceleration)
        {
            CheckGround();
            AutoMove(currentMaxSpeed, currentAcceleration);
        }

        public void CheckGround()
        {
            Vector2 origin = transform.position;
            var hit = Physics2D.Raycast(origin, Vector2.down, _playerSettings.RayLength, _playerSettings.GroundLayer);
            IsGrounded = hit.collider != null;
            GroundNormal = IsGrounded ? hit.normal : Vector2.up;
        }

        private void AutoMove(float currentMaxSpeed, float currentAcceleration)
        {
            if (IsGrounded && _rb.linearVelocity.x < currentMaxSpeed)
                _rb.AddForce(Vector2.right * currentAcceleration);
        }

        public void Jump(float jumpForce)
        {
            if (!IsGrounded) return;
            
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        public void Boost(float boostForce)
        {
            if (!IsGrounded) return;
            
            _rb.AddForce(Vector2.right * boostForce, ForceMode2D.Impulse);
        }

        public void ResetVelocity()
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }
}