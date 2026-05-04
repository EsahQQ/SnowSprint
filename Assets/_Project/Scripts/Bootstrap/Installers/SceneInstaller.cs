using UnityEngine;
using Zenject;

namespace _Project.Scripts.Bootstrap.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();
        }
    }
}