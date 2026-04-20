using System;

namespace _Project.Scripts.Features.Player.Provider
{
    public class PlayerProvider : IPlayerProvider
    {
        public PlayerController Player { get; private set; }
        public event Action<PlayerController> OnPlayerRegistered;
        public void RegisterPlayer(PlayerController player)
        {
            Player = player;
            OnPlayerRegistered?.Invoke(player);
        }
    }
}