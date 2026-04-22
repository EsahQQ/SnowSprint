using System;
using System.Collections.Generic;

namespace _Project.Scripts.Features.Player.Provider
{
    public interface IPlayerProvider
    {
        PlayerController LocalPlayer { get; } 
        IReadOnlyList<PlayerController> AllPlayers { get; }
        event Action<PlayerController> OnLocalPlayerRegistered;
        
        void RegisterPlayer(PlayerController player);
        void UnregisterPlayer(PlayerController player);
    }
}