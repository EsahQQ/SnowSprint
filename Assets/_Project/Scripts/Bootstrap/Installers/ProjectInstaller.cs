using _Project.Scripts.Bootstrap.InitPipeline.Initializers.Global;
using _Project.Scripts.Bootstrap.InitPipeline.Queue;
using _Project.Scripts.Bootstrap.InitPipeline.Tasks;
using _Project.Scripts.Bootstrap.InitPipeline.Tasks.GlobalTasks;
using _Project.Scripts.Features.Network;
using _Project.Scripts.Infrastructure.SceneManagement;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            BindInitPipeline();
            
            Container.Bind<INetworkSessionService>().To<MockNetworkSessionService>().AsSingle();
        }

        private void BindInitPipeline()
        {
            Container.Bind<IGlobalInitializer>().To<GlobalInitializer>().AsSingle();
            Container.Bind<IInitQueue>().To<InitQueue>().AsSingle();
            
            Container.Bind<IInitTask>().To<LoadProfileTask>().AsSingle();
            Container.Bind<IInitTask>().To<LoadLevelsTask>().AsSingle();
        }
    }
}