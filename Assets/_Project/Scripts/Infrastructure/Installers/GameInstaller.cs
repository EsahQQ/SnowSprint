using _Project.Scripts.Gameplay.Level;
using _Project.Scripts.Infrastructure.Managers;
using _Project.Scripts.Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameManager gameManagerPrefab;
        [SerializeField] private FinishTrigger finishTrigger;

        public override void InstallBindings()
        {
            Container.Bind<IPlayerDataService>().To<PlayerDataService>().AsSingle();
            Container.Bind<FinishTrigger>().FromComponentInHierarchy(finishTrigger).AsSingle();
            Container.Bind<GameManager>().FromComponentInNewPrefab(gameManagerPrefab).AsSingle().NonLazy();
        }
    }
}