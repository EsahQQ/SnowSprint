using _Project.Scripts.Features.Player.Settings;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Features.Player
{
    public class PlayerVisuals : MonoBehaviour
    {
        [SerializeField] private Transform visualSprite;
        [Inject] private PlayerSettings _playerSettings;
        public void Initialize() {}
        
        public void FixedTick(float fixedDt, Vector2 groundNormal)
        {
            RotateSprite(fixedDt, groundNormal);
        }

        private void RotateSprite(float fixedDt, Vector2 groundNormal)
        {
            var normal = groundNormal;

            var targetAngle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f;
            var targetRotation = Quaternion.Euler(0, 0, targetAngle);

            visualSprite.rotation = Quaternion.Lerp(visualSprite.rotation, targetRotation, fixedDt * _playerSettings.RotationSpeed);
        }
    }
}