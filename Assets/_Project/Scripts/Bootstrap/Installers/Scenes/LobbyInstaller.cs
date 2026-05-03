using _Project.Scripts.Features.Network.Lobby;
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
            Container.Bind<ILobbyView>().FromInstance(_lobbyView).AsSingle();
            Container.BindInterfacesAndSelfTo<LobbyController>().AsSingle();
        }
    }
}