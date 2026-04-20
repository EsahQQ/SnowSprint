using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Bootstrap.InitPipeline.Queue
{
    public interface IInitQueue
    {
        UniTask RunAsync();
    }
}