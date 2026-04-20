using System;

namespace _Project.Scripts.Features.Player.Provider
{
    public interface IPlayerProvider
    {
        PlayerController Player { get; }
        event Action<PlayerController> OnPlayerRegistered;
        void RegisterPlayer(PlayerController player);
    }
}