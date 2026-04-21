using UnityEngine;

namespace _Project.Scripts.Features.Player.PlayerInput
{
    public abstract class AbstractPlayerInput : MonoBehaviour
    {
        protected bool JumpTriggered;
        protected bool BoostTriggered;
        
        public bool GetJumpInput() => JumpTriggered;
        public bool GetBoostInput() => BoostTriggered;
        
        public abstract void Tick(); 
    }
}