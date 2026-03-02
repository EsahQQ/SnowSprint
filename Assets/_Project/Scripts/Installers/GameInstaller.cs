using _Project.Scripts.Core;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Installers
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