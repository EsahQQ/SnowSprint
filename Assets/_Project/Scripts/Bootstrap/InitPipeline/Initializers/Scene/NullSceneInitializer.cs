using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace _Project.Scripts.Bootstrap.InitPipeline.Initializers.Scene
{
    [UsedImplicitly]
    public class NullSceneInitializer : ISceneInitializer
    {
        public UniTask<bool> EnsureInitializedAsync() => UniTask.FromResult(true);
    }
}