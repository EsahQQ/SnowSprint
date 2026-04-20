using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Bootstrap.InitPipeline.Tasks
{
    public interface IInitTask
    {
        int Priority { get; }
        UniTask Execute();
    }
}