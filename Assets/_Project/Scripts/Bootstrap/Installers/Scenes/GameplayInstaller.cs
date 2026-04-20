using _Project.Scripts.Bootstrap.EntryPoint;
using _Project.Scripts.Bootstrap.InitPipeline.Initializers.Scene;
using _Project.Scripts.Features.AppStates;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers.Scenes
{
    public class GameplayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindScenePipeline();
            Container.BindInterfacesAndSelfTo<SceneEntryPoint<GameState>>().AsSingle();
        }

        private void BindScenePipeline()
        {
            Container.Bind<ISceneInitializer>().To<NullSceneInitializer>().AsSingle();
        }
    }
}