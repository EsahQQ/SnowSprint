using UnityEngine;

namespace _Project.Scripts.Features.Gameplay.Player
{
    public class PlayerVisuals : MonoBehaviour
    {
        [SerializeField] private Transform visualSprite;
        [SerializeField] private float rotationSpeed = 10f;

        private PlayerPhysics _physics;
        private void Awake()
        {
            _physics = GetComponent<PlayerPhysics>();
        }

        private void FixedUpdate()
        {
            RotateSprite();
        }

        private void RotateSprite()
        {
            var normal = _physics.GroundNormal;
            
            var targetAngle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f;
            var targetRotation = Quaternion.Euler(0, 0, targetAngle);
            
            visualSprite.rotation = Quaternion.Lerp(visualSprite.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }
}