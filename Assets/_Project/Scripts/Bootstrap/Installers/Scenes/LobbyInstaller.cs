using _Project.Scripts.Bootstrap.EntryPoint;
using _Project.Scripts.Features.AppStates.Network;
using _Project.Scripts.Features.UI.Lobby;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers.Scenes
{
    public class LobbyInstaller : MonoInstaller
    {
        [SerializeField] private LobbyView _lobbyView;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SceneEntryPoint<LobbyState>>().AsSingle();
            
            Container.Bind<ILobbyView>().FromInstance(_lobbyView).AsSingle();
        }
    }
}