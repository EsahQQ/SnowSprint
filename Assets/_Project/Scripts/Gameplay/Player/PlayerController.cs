using UnityEngine;

namespace _Project.Scripts.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float maxSpeed = 15f;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float boostForce = 15f;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private Transform visualSprite; 

        [Header("Input Settings")]
        [SerializeField] private float minSwipeLength = 100f; 

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float rayLength = 1.5f;

        private Rigidbody2D _rb;
        private bool _isGrounded;
        private Vector2 _groundNormal;

        private Vector2 _touchStartPos;
        private bool _isSwiping;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            CheckGround();
            Move();
            RotateVisuals();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && _isGrounded) 
                Jump();
            if ((Input.GetKeyDown(KeyCode.DownArrow)/* || Input.GetMouseButtonDown(0) */) && _isGrounded)
                Boost();

            if (Input.touchCount == 2)
            {
                var t1 = Input.GetTouch(0);
                var t2 = Input.GetTouch(1);

                var centerPos = (t1.position + t2.position) / 2f;

                if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began)
                {
                    _touchStartPos = centerPos;
                    _isSwiping = true;
                }
                else if ((t1.phase == TouchPhase.Ended || t2.phase == TouchPhase.Ended) && _isSwiping)
                {
                    var swipeVector = centerPos - _touchStartPos;

                    if (swipeVector.magnitude > minSwipeLength
                        && Mathf.Abs(swipeVector.y) > Mathf.Abs(swipeVector.x))
                    {
                        if (swipeVector.y > 0)
                        {
                            if (_isGrounded) Jump();
                        }
                        else
                        {
                            if (_isGrounded) Boost();
                        }
                    }
                    
                    _isSwiping = false;
                }
            }
            else
                _isSwiping = false;
        }

        private void CheckGround()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);
            _isGrounded = hit.collider != null;

            _groundNormal = _isGrounded ? hit.normal : Vector2.up;
            
            Debug.DrawRay(transform.position, Vector2.down * rayLength, _isGrounded ? Color.green : Color.red);
        }

        private void Move()
        {
            if (_isGrounded && _rb.linearVelocity.x < maxSpeed)
                _rb.AddForce(Vector2.right * acceleration);
        }

        private void Jump()
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void Boost() => _rb.AddForce(Vector2.right * boostForce, ForceMode2D.Impulse);

        private void RotateVisuals()
        {
            var targetAngle = Mathf.Atan2(_groundNormal.y, _groundNormal.x) * Mathf.Rad2Deg - 90f;
            var targetRotation = Quaternion.Euler(0, 0, targetAngle);
            visualSprite.rotation = Quaternion.Lerp(visualSprite.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }
}