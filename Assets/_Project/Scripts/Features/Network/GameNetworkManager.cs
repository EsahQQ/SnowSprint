using System;
using Mirror;
using UnityEngine;

namespace _Project.Scripts.Features.Network
{
    public class GameNetworkManager : NetworkManager
    {
        public static event Action<NetworkConnectionToClient> OnServerAddPlayerCallback;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            OnServerAddPlayerCallback?.Invoke(conn);
        }
    }
}