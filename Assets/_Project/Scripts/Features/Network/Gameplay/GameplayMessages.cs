using Mirror;

namespace _Project.Scripts.Features.Network.Gameplay
{
    public struct RaceStartMessage : NetworkMessage { }

    public struct RaceFinishMessage : NetworkMessage
    {
        public int CoinsEarned;
    }
}