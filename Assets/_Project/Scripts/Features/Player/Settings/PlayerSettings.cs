using UnityEngine;

namespace _Project.Scripts.Features.Player.Settings
{
    public class PlayerSettings : ScriptableObject
    {
        [Header("Physics")]
        public LayerMask GroundLayer;
        public float RayLength = 1.5f;
        
        [Header("Stats")]
        public float BaseMaxSpeed = 10f;
        public float BaseAcceleration = 2f;
        public float BaseBoostForce = 2f;
        public float BaseJumpForce = 5f;
        
        [Header("Visuals")]
        public float RotationSpeed = 10f;
    }
}