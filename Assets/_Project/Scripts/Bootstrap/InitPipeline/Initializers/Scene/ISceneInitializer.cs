using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Bootstrap.InitPipeline.Initializers.Scene
{
    public interface ISceneInitializer
    {
        UniTask<bool> EnsureInitializedAsync();
    }
}