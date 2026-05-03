using Mirror;

namespace _Project.Scripts.Features.Player.Services
{
    public struct GetProfileRequest : NetworkMessage { }

    public struct ProfileDataResponse : NetworkMessage
    {
        public int Coins;
        public string[] UnlockedUpgrades;
    }
    
    public struct BuyUpgradeRequest : NetworkMessage
    {
        public string UpgradeId;
        public int Price;
    }
}