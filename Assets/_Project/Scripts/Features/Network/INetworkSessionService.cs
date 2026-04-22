using System;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Features.Network
{
    public interface INetworkSessionService
    {
        bool IsHost { get; }
        bool IsConnected { get; }
        
        event Action OnAllPlayersReady;

        void StartHost();
        void StartClient();
        void SetLocalPlayerReady(bool isReady);
        
        UniTask QuickJoinOrCreateAsync(); 
    }
}