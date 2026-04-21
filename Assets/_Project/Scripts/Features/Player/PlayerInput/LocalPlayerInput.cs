using UnityEngine;

namespace _Project.Scripts.Features.Player.PlayerInput
{
    public class LocalPlayerInput : AbstractPlayerInput
    {
        [SerializeField] private float minSwipeLength = 100f;
        
        private Vector2 _touchStartPos;
        private bool _isSwiping;
        
        public override void Tick()
        {
            JumpTriggered = false;
            BoostTriggered = false;

            HandlePCInput();
            HandleTouchInput();
        }

        private void HandlePCInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) JumpTriggered = true;
            if (Input.GetKeyDown(KeyCode.DownArrow)) BoostTriggered = true;
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

                    if (swipeVector.magnitude > minSwipeLength && Mathf.Abs(swipeVector.y) > Mathf.Abs(swipeVector.x))
                    {
                        if (swipeVector.y > 0) JumpTriggered = true;
                        else BoostTriggered = true;
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