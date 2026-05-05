using Mirror;

namespace _Project.Scripts.Features.Network.Server.Auth
{
    public interface IRaceRewardService
    {
        void GrantCoinsToPlayer(NetworkConnectionToClient conn, int amount);
    }
}