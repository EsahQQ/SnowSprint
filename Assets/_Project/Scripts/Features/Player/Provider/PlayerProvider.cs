using System;
using System.Collections.Generic;

namespace _Project.Scripts.Features.Player.Provider
{
    public class PlayerProvider : IPlayerProvider
    {
        public PlayerController LocalPlayer { get; private set; }
        private readonly List<PlayerController> _allPlayers = new();
        public IReadOnlyList<PlayerController> AllPlayers => _allPlayers;
        
        public event Action<PlayerController> OnLocalPlayerRegistered;
        public event Action<PlayerController> OnAnyPlayerRegistered;

        public void RegisterPlayer(PlayerController player)
        {
            _allPlayers.Add(player);
            OnAnyPlayerRegistered?.Invoke(player);

            if (player.isLocalPlayer)
            {
                LocalPlayer = player;
                OnLocalPlayerRegistered?.Invoke(player);
            }
        }

        public void UnregisterPlayer(PlayerController player)
        {
            _allPlayers.Remove(player);
            if (player == LocalPlayer) LocalPlayer = null;
        }
    }
}