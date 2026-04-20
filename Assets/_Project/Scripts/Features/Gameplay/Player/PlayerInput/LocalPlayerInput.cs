using UnityEngine;

namespace _Project.Scripts.Features.Gameplay.Player.PlayerInput
{
    public class LocalPlayerInput : MonoBehaviour, IPlayerInput
    {
        [SerializeField] private float minSwipeLength = 100f;
        
        private Vector2 _touchStartPos;
        private bool _isSwiping;
        private bool _jumpTriggered;
        private bool _boostTriggered;

        private void Update()
        {
            _jumpTriggered = false;
            _boostTriggered = false;

            HandlePCInput();
            HandleTouchInput();
        }

        public bool GetJumpInput() => _jumpTriggered;
        public bool GetBoostInput() => _boostTriggered;

        private void HandlePCInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) _jumpTriggered = true;
            if (Input.GetKeyDown(KeyCode.DownArrow)) _boostTriggered = true;
        }

        private void HandleTouchInput()
        {
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
                        if (swipeVector.y > 0) _jumpTriggered = true;
                        else _boostTriggered = true;
                    }
                    _isSwiping = false;
                }
            }
            else
            {
                _isSwiping = false;
            }
        }
    }
}