using UnityEngine;

namespace _Project.Scripts.Features.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerPhysics : MonoBehaviour
    {
        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float rayLength = 1.5f;

        private PlayerStatsHandler _stats;
        private Rigidbody2D _rb;
        private bool _isActive = true;

        public bool IsGrounded { get; private set; }
        public Vector2 GroundNormal { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _stats = GetComponent<PlayerStatsHandler>();
        }

        private void FixedUpdate()
        {
            if (!_isActive) return; 

            CheckGround();
            AutoMove();
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            if (!isActive) _rb.linearVelocity = Vector2.zero; 
        }

        private void CheckGround()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);
            IsGrounded = hit.collider != null;
            GroundNormal = IsGrounded ? hit.normal : Vector2.up;
            
            Debug.DrawRay(transform.position, Vector2.down * rayLength, IsGrounded ? Color.green : Color.red);
        }

        private void AutoMove()
        {
            if (IsGrounded && _rb.linearVelocity.x < _stats.CurrentMaxSpeed)
            {
                _rb.AddForce(Vector2.right * _stats.CurrentAcceleration);
            }
        }

        public void Jump()
        {
            if (!IsGrounded) return;
            
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
            _rb.AddForce(Vector2.up * _stats.CurrentJumpForce, ForceMode2D.Impulse);
        }

        public void Boost()
        {
            if (!IsGrounded) return;
            
            _rb.AddForce(Vector2.right * _stats.CurrentBoostForce, ForceMode2D.Impulse);
        }
    }
}