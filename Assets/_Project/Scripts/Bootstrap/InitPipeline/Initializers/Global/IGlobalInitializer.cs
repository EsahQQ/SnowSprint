using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Bootstrap.InitPipeline.Initializers.Global
{
    public interface IGlobalInitializer
    {
        UniTask<bool> EnsureInitializedAsync();
    }
}