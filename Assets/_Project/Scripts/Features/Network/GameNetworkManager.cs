using System;
using _Project.Scripts.Features.SceneConstants;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Features.Network
{
    public class GameNetworkManager : NetworkManager
    {
        public override void OnClientSceneChanged()
        {
            base.OnClientSceneChanged();
            
            if (SceneManager.GetActiveScene().name == SceneNames.Game)
            {
                if (NetworkClient.isConnected)
                {
                    NetworkClient.AddPlayer();
                }
            }
        }
    }
}